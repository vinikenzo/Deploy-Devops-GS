using ecoorbit_dotnet.Domain.Entities;
using ecoorbit_dotnet.Infrastructure.Data;
using ecoorbit_dotnet.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ecoorbit_dotnet.Infrastructure.Repositories.Implementations;

public class FireDetectionResultRepository : IFireDetectionResultRepository
{
    private readonly AppDbContext _context;

    public FireDetectionResultRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<FireDetectionResult?> GetByIdAsync(Guid id) =>
        await _context.FireDetectionResults
            .Include(f => f.SatelliteImage)
            .FirstOrDefaultAsync(f => f.Id == id);

    public async Task<FireDetectionResult?> GetBySatelliteImageIdAsync(Guid satelliteImageId) =>
        await _context.FireDetectionResults
            .Include(f => f.SatelliteImage)
            .FirstOrDefaultAsync(f => f.SatelliteImageId == satelliteImageId);

    public async Task<IEnumerable<FireDetectionResult>> GetAllAsync() =>
        await _context.FireDetectionResults
            .Include(f => f.SatelliteImage)
            .ToListAsync();

    public async Task AddAsync(FireDetectionResult result)
    {
        await _context.FireDetectionResults.AddAsync(result);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(FireDetectionResult result)
    {
        _context.FireDetectionResults.Update(result);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(FireDetectionResult result)
    {
        _context.FireDetectionResults.Remove(result);
        await _context.SaveChangesAsync();
    }
}