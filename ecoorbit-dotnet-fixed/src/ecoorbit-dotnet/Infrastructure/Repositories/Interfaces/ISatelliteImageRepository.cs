using ecoorbit_dotnet.Domain.Entities;

namespace ecoorbit_dotnet.Infrastructure.Repositories.Interfaces;

public interface ISatelliteImageRepository
{
    Task<SatelliteImage?> GetByIdAsync(Guid id);
    Task<IEnumerable<SatelliteImage>> GetAllAsync();
    Task<IEnumerable<SatelliteImage>> GetByUserIdAsync(Guid userId);
    Task AddAsync(SatelliteImage image);
    Task UpdateAsync(SatelliteImage image);
    Task DeleteAsync(SatelliteImage image);
}