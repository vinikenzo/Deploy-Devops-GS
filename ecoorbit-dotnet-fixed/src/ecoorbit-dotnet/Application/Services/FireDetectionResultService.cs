using ecoorbit_dotnet.Application.DTOs.FireDetectionResult;
using ecoorbit_dotnet.Application.Interfaces;
using ecoorbit_dotnet.Domain.Entities;
using ecoorbit_dotnet.Infrastructure.Repositories.Interfaces;

namespace ecoorbit_dotnet.Application.Services;

public class FireDetectionResultService : IFireDetectionResultService
{
    private readonly IFireDetectionResultRepository _repository;
    private readonly ISatelliteImageRepository _imageRepository;

    public FireDetectionResultService(IFireDetectionResultRepository repository, ISatelliteImageRepository imageRepository)
    {
        _repository = repository;
        _imageRepository = imageRepository;
    }

    public async Task<IEnumerable<FireDetectionResultResponseDto>> GetAllAsync()
    {
        var results = await _repository.GetAllAsync();
        return results.Select(MapToDto);
    }

    public async Task<FireDetectionResultResponseDto> GetByIdAsync(Guid id)
    {
        var result = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Detection result {id} not found.");
        return MapToDto(result);
    }

    public async Task<FireDetectionResultResponseDto> GetBySatelliteImageIdAsync(Guid satelliteImageId)
    {
        var result = await _repository.GetBySatelliteImageIdAsync(satelliteImageId)
            ?? throw new KeyNotFoundException($"No detection result for image {satelliteImageId}.");
        return MapToDto(result);
    }

    public async Task<FireDetectionResultResponseDto> CreateAsync(CreateFireDetectionResultDto dto)
    {
        var image = await _imageRepository.GetByIdAsync(dto.SatelliteImageId)
            ?? throw new KeyNotFoundException($"Satellite image {dto.SatelliteImageId} not found.");

        var existing = await _repository.GetBySatelliteImageIdAsync(dto.SatelliteImageId);
        if (existing is not null)
            throw new InvalidOperationException("A detection result already exists for this image.");

        var result = new FireDetectionResult
        {
            Id = Guid.NewGuid(),
            FireDetected = dto.FireDetected,
            RiskLevel = dto.RiskLevel,
            ConfidenceScore = dto.ConfidenceScore,
            Notes = dto.Notes,
            SatelliteImageId = dto.SatelliteImageId
        };

        await _repository.AddAsync(result);
        result.SatelliteImage = image;
        return MapToDto(result);
    }

    public async Task<FireDetectionResultResponseDto> UpdateAsync(Guid id, UpdateFireDetectionResultDto dto)
    {
        var result = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Detection result {id} not found.");

        result.FireDetected = dto.FireDetected;
        result.RiskLevel = dto.RiskLevel;
        result.ConfidenceScore = dto.ConfidenceScore;
        result.Notes = dto.Notes;

        await _repository.UpdateAsync(result);
        return MapToDto(result);
    }

    public async Task DeleteAsync(Guid id)
    {
        var result = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Detection result {id} not found.");
        await _repository.DeleteAsync(result);
    }

    private static FireDetectionResultResponseDto MapToDto(FireDetectionResult result) => new()
    {
        Id = result.Id,
        FireDetected = result.FireDetected,
        RiskLevel = result.RiskLevel,
        RiskLevelName = result.RiskLevel.ToString(),
        ConfidenceScore = result.ConfidenceScore,
        Notes = result.Notes,
        AnalyzedAt = result.AnalyzedAt,
        SatelliteImageId = result.SatelliteImageId,
        Region = result.SatelliteImage?.Region ?? string.Empty
    };
}
