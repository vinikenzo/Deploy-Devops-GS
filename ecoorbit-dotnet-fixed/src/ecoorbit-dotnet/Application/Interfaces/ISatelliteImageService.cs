using ecoorbit_dotnet.Application.DTOs.SatelliteImage;

namespace ecoorbit_dotnet.Application.Interfaces;

public interface ISatelliteImageService
{
    Task<IEnumerable<SatelliteImageResponseDto>> GetAllAsync();
    Task<SatelliteImageResponseDto> GetByIdAsync(Guid id);
    Task<IEnumerable<SatelliteImageResponseDto>> GetByUserIdAsync(Guid userId);
    Task<SatelliteImageResponseDto> CreateAsync(CreateSatelliteImageDto dto, Guid userId);
    Task<SatelliteImageResponseDto> UpdateAsync(Guid id, UpdateSatelliteImageDto dto);
    Task DeleteAsync(Guid id);
}
