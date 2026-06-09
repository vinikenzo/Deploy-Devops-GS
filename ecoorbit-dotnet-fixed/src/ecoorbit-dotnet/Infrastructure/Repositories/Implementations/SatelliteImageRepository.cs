using ecoorbit_dotnet.Domain.Entities;
using ecoorbit_dotnet.Infrastructure.Data;
using ecoorbit_dotnet.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ecoorbit_dotnet.Infrastructure.Repositories.Implementations;

public class SatelliteImageRepository : ISatelliteImageRepository
{
    private readonly AppDbContext _context;

    public SatelliteImageRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<SatelliteImage?> GetByIdAsync(Guid id) =>
        await _context.SatelliteImages
            .Include(s => s.User)
            .Include(s => s.DetectionResult)
            .FirstOrDefaultAsync(s => s.Id == id);

    public async Task<IEnumerable<SatelliteImage>> GetAllAsync() =>
        await _context.SatelliteImages
            .Include(s => s.User)
            .Include(s => s.DetectionResult)
            .ToListAsync();

    public async Task<IEnumerable<SatelliteImage>> GetByUserIdAsync(Guid userId) =>
        await _context.SatelliteImages
            .Include(s => s.DetectionResult)
            .Where(s => s.UserId == userId)
            .ToListAsync();

    public async Task AddAsync(SatelliteImage image)
    {
        await _context.SatelliteImages.AddAsync(image);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(SatelliteImage image)
    {
        _context.SatelliteImages.Update(image);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(SatelliteImage image)
    {
        _context.SatelliteImages.Remove(image);
        await _context.SaveChangesAsync();
    }
}