using ecoorbit_dotnet.Application.DTOs.FireDetectionResult;

namespace ecoorbit_dotnet.Application.Interfaces;

public interface IFireDetectionResultService
{
    Task<IEnumerable<FireDetectionResultResponseDto>> GetAllAsync();
    Task<FireDetectionResultResponseDto> GetByIdAsync(Guid id);
    Task<FireDetectionResultResponseDto> GetBySatelliteImageIdAsync(Guid satelliteImageId);
    Task<FireDetectionResultResponseDto> CreateAsync(CreateFireDetectionResultDto dto);
    Task<FireDetectionResultResponseDto> UpdateAsync(Guid id, UpdateFireDetectionResultDto dto);
    Task DeleteAsync(Guid id);
}
