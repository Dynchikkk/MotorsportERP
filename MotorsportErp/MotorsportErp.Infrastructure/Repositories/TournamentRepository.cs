using Microsoft.EntityFrameworkCore;
using MotorsportErp.Application.Interfaces.Repositories;
using MotorsportErp.Domain.Tournaments;
using MotorsportErp.Infrastructure.Persistence;

namespace MotorsportErp.Infrastructure.Repositories;

public class TournamentRepository : ITournamentRepository
{
    private readonly AppDbContext _context;

    public TournamentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Tournament?> GetByIdAsync(Guid id)
    {
        return await _context.Tournaments
            .Include(t => t.Applications)
                .ThenInclude(a => a.User)
            .Include(t => t.Applications)
                .ThenInclude(a => a.Car)
            .Include(t => t.Results)
            .Include(t => t.Organizers)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<List<Tournament>> GetAllAsync()
    {
        return await _context.Tournaments
            .Include(t => t.Applications)
            .ToListAsync();
    }

    public async Task<List<Tournament>> GetByStatusAsync(TournamentStatus status)
    {
        return await _context.Tournaments
            .Where(t => t.Status == status)
            .Include(t => t.Applications)
            .ToListAsync();
    }

    public async Task AddAsync(Tournament entity)
    {
        _ = await _context.Tournaments.AddAsync(entity);
        _ = await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Tournament entity)
    {
        _ = _context.Tournaments.Update(entity);
        _ = await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Tournament entity)
    {
        _ = _context.Tournaments.Remove(entity);
        _ = await _context.SaveChangesAsync();
    }

    public async Task AddApplicationAsync(TournamentApplication application)
    {
        _ = await _context.TournamentApplications.AddAsync(application);
        _ = await _context.SaveChangesAsync();
    }

    public async Task<TournamentApplication?> GetApplicationByIdAsync(Guid id)
    {
        return await _context.TournamentApplications
            .Include(a => a.Tournament)
                .ThenInclude(t => t.Applications)
            .Include(a => a.Tournament)
                .ThenInclude(t => t.Organizers)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<List<TournamentApplication>> GetApplicationsByTournamentIdAsync(Guid tournamentId)
    {
        return await _context.TournamentApplications
            .Where(a => a.TournamentId == tournamentId)
            .ToListAsync();
    }

    public async Task AddResultAsync(TournamentResult result)
    {
        _ = await _context.TournamentResults.AddAsync(result);
        _ = await _context.SaveChangesAsync();
    }

    public async Task<bool> HasUserAppliedAsync(Guid tournamentId, Guid userId)
    {
        return await _context.TournamentApplications
            .AnyAsync(a => a.TournamentId == tournamentId && a.UserId == userId);
    }
}