using Microsoft.EntityFrameworkCore;
using MotorsportErp.Application.Interfaces.Repositories;
using MotorsportErp.Domain.Cars;
using MotorsportErp.Domain.Tournaments;
using MotorsportErp.Infrastructure.Persistence;

namespace MotorsportErp.Infrastructure.Repositories;

public class CarRepository : ICarRepository
{
    private readonly AppDbContext _context;

    public CarRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Car?> GetByIdAsync(Guid id)
    {
        return await _context.Cars
            .Include(c => c.Applications)
                .ThenInclude(a => a.Tournament)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<Car>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Cars
            .Where(c => c.OwnerId == userId)
            .ToListAsync();
    }

    public async Task AddAsync(Car entity)
    {
        _ = await _context.Cars.AddAsync(entity);
        _ = await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Car entity)
    {
        _ = _context.Cars.Update(entity);
        _ = await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Car entity)
    {
        _ = _context.Cars.Remove(entity);
        _ = await _context.SaveChangesAsync();
    }

    public async Task<bool> HasActiveApplicationsAsync(Guid carId)
    {
        return await _context.TournamentApplications
            .AnyAsync(a => a.CarId == carId &&
                           a.Tournament.Status != TournamentStatus.Finished &&
                           a.Tournament.Status != TournamentStatus.Cancelled);
    }
}