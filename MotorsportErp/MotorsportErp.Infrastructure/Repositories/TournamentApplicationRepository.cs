using Microsoft.EntityFrameworkCore;
using MotorsportErp.Application.Interfaces.Repositories;
using MotorsportErp.Domain.Tournaments;
using MotorsportErp.Infrastructure.Persistence;

namespace MotorsportErp.Infrastructure.Repositories;

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
}