using Microsoft.EntityFrameworkCore;
using MotorsportErp.Domain.Tournaments;

namespace MotorsportErp.Infrastructure.Persistence.Repositories;

public class TournamentApplicationRepository : ITournamentApplicationRepository
{
    private readonly AppDbContext _context;

    public TournamentApplicationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TournamentApplication?> GetByIdAsync(Guid id)
    {
        return await _context.TournamentApplications
            .Include(a => a.Tournament)
                .ThenInclude(t => t.Applications)
            .Include(a => a.Tournament)
                .ThenInclude(t => t.Organizers)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task UpdateAsync(TournamentApplication application)
    {
        _ = _context.TournamentApplications.Update(application);
        _ = await _context.SaveChangesAsync();
    }

    public async Task AddAsync(TournamentApplication application)
    {
        _ = await _context.TournamentApplications.AddAsync(application);
        _ = await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Guid userId, Guid tournamentId)
    {
        return await _context.TournamentApplications
            .AnyAsync(a => a.UserId == userId && a.TournamentId == tournamentId);
    }

    public async Task<int> GetApprovedCountAsync(Guid tournamentId)
    {
        return await _context.TournamentApplications
            .CountAsync(a => a.TournamentId == tournamentId &&
                             a.Status == TournamentApplicationStatus.Approved);
    }

    public async Task DeleteAsync(TournamentApplication entity)
    {
        _ = _context.TournamentApplications.Remove(entity);
        _ = await _context.SaveChangesAsync();
    }
}