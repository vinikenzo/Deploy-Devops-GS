using ecoorbit_dotnet.Application.DTOs.SatelliteImage;
using ecoorbit_dotnet.Application.Interfaces;
using ecoorbit_dotnet.Domain.Entities;
using ecoorbit_dotnet.Domain.Enums;
using ecoorbit_dotnet.Infrastructure.Repositories.Interfaces;

namespace ecoorbit_dotnet.Application.Services;

public class SatelliteImageService : ISatelliteImageService
{
    private readonly ISatelliteImageRepository _repository;
    private readonly IUserRepository _userRepository;
    private readonly IFireDetectionResultRepository _resultRepository;
    private readonly IFlaskAnalysisClient _flaskClient;
    private readonly ILogger<SatelliteImageService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public SatelliteImageService(
        ISatelliteImageRepository repository,
        IUserRepository userRepository,
        IFireDetectionResultRepository resultRepository,
        IFlaskAnalysisClient flaskClient,
        ILogger<SatelliteImageService> logger,
        IServiceScopeFactory scopeFactory)
    {
        _repository = repository;
        _userRepository = userRepository;
        _resultRepository = resultRepository;
        _flaskClient = flaskClient;
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    public async Task<IEnumerable<SatelliteImageResponseDto>> GetAllAsync()
    {
        var images = await _repository.GetAllAsync();
        return images.Select(MapToDto);
    }

    public async Task<SatelliteImageResponseDto> GetByIdAsync(Guid id)
    {
        var image = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Satellite image {id} not found.");
        return MapToDto(image);
    }

    public async Task<IEnumerable<SatelliteImageResponseDto>> GetByUserIdAsync(Guid userId)
    {
        var images = await _repository.GetByUserIdAsync(userId);
        return images.Select(MapToDto);
    }

    public async Task<SatelliteImageResponseDto> CreateAsync(CreateSatelliteImageDto dto, Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException($"User {userId} not found.");

        var delta = 1.0;
        var imageUrl = $"https://wvs.earthdata.nasa.gov/api/v1/snapshot" +
                       $"?REQUEST=GetSnapshot" +
                       $"&TIME={dto.CapturedAt:yyyy-MM-dd}" +
                       $"&BBOX={dto.Latitude - delta},{dto.Longitude - delta},{dto.Latitude + delta},{dto.Longitude + delta}" +
                       $"&CRS=EPSG:4326" +
                       $"&LAYERS=MODIS_Terra_CorrectedReflectance_TrueColor" +
                       $"&WRAP=DAY&WIDTH=512&HEIGHT=512&FORMAT=image/jpeg";

        var image = new SatelliteImage
        {
            Id = Guid.NewGuid(),
            ImageUrl = imageUrl,
            Region = dto.Region,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            CapturedAt = dto.CapturedAt,
            UserId = userId
        };

        await _repository.AddAsync(image);
        image.User = user;

        _ = Task.Run(async () => await RunAnalysisAsync(image));

        return MapToDto(image);
    }

    public async Task<SatelliteImageResponseDto> UpdateAsync(Guid id, UpdateSatelliteImageDto dto)
    {
        var image = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Satellite image {id} not found.");

        image.Region = dto.Region;
        image.Latitude = dto.Latitude;
        image.Longitude = dto.Longitude;
        image.CapturedAt = dto.CapturedAt;

        await _repository.UpdateAsync(image);
        return MapToDto(image);
    }

    public async Task DeleteAsync(Guid id)
    {
        var image = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Satellite image {id} not found.");
        await _repository.DeleteAsync(image);
    }

    private async Task RunAnalysisAsync(SatelliteImage image)
    {
        try
        {
            var result = await _flaskClient.AnalyzeAsync(image.Latitude, image.Longitude, image.CapturedAt);

            if (result is null)
            {
                _logger.LogWarning("Flask returned no valid result for image {Id}", image.Id);
                return;
            }

            var confidence = result.ConfidencePercentage / 100.0;

            var riskLevel = result.FireDetected switch
            {
                false => FireRiskLevel.None,
                true when confidence < 0.5 => FireRiskLevel.Low,
                true when confidence < 0.7 => FireRiskLevel.Medium,
                true when confidence < 0.85 => FireRiskLevel.High,
                _ => FireRiskLevel.Critical
            };

            var detectionResult = new FireDetectionResult
            {
                Id = Guid.NewGuid(),
                SatelliteImageId = image.Id,
                FireDetected = result.FireDetected,
                RiskLevel = riskLevel,
                ConfidenceScore = confidence,
                Notes = $"Análise automática via eco-orbit Flask — {DateTime.UtcNow:yyyy-MM-dd}"
            };

            using var scope = _scopeFactory.CreateScope();
            var resultRepo = scope.ServiceProvider.GetRequiredService<IFireDetectionResultRepository>();
            await resultRepo.AddAsync(detectionResult);

            _logger.LogInformation("Detection saved for image {Id}: fire={Fire}, risk={Risk}",
                image.Id, result.FireDetected, riskLevel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Analysis pipeline failed for image {Id}", image.Id);
        }
    }

    private static SatelliteImageResponseDto MapToDto(SatelliteImage image) => new()
    {
        Id = image.Id,
        ImageUrl = image.ImageUrl,
        Region = image.Region,
        Latitude = image.Latitude,
        Longitude = image.Longitude,
        CapturedAt = image.CapturedAt,
        SubmittedAt = image.SubmittedAt,
        UserId = image.UserId,
        UserName = image.User?.Name ?? string.Empty,
        HasDetectionResult = image.DetectionResult is not null
    };
}
