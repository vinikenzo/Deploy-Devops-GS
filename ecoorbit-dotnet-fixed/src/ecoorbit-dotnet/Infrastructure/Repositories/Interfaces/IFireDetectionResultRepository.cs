using ecoorbit_dotnet.Domain.Entities;

namespace ecoorbit_dotnet.Infrastructure.Repositories.Interfaces;

public interface IFireDetectionResultRepository
{
    Task<FireDetectionResult?> GetByIdAsync(Guid id);
    Task<FireDetectionResult?> GetBySatelliteImageIdAsync(Guid satelliteImageId);
    Task<IEnumerable<FireDetectionResult>> GetAllAsync();
    Task AddAsync(FireDetectionResult result);
    Task UpdateAsync(FireDetectionResult result);
    Task DeleteAsync(FireDetectionResult result);
}