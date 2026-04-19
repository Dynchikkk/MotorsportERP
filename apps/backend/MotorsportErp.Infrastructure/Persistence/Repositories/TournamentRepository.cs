using Microsoft.EntityFrameworkCore;
using MotorsportErp.Application.Common.Interfaces.Repositories;
using MotorsportErp.Application.Features.Tournaments.Contracts;
using MotorsportErp.Domain.Tournaments;
using MotorsportErp.Infrastructure.Extensions;
using System.Linq.Expressions;

namespace MotorsportErp.Infrastructure.Persistence.Repositories;

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
            .Include(t => t.Track)
                .ThenInclude(track => track.Votes)
            .Include(t => t.Track)
                .ThenInclude(track => track.Photos)
            .Include(t => t.Applications)
                .ThenInclude(a => a.User)
                    .ThenInclude(u => u.Avatar)
            .Include(t => t.Applications)
                .ThenInclude(a => a.Car)
                    .ThenInclude(c => c.Photos)
            .Include(t => t.Photos)
            .Include(t => t.Results)
                .ThenInclude(r => r.User)
                    .ThenInclude(u => u.Avatar)
            .Include(t => t.Organizers)
                .ThenInclude(o => o.User)
                    .ThenInclude(u => u.Avatar)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<(List<Tournament> Items, int TotalCount)> GetPagedAsync(
            Expression<Func<Tournament, bool>>? filter,
            int page,
            int pageSize)
    {
        var query = _context.Tournaments
            .Include(t => t.Applications)
            .Include(t => t.Photos)
            .Include(t => t.Track)
            .AsQueryable();

        if (filter != null)
        {
            query = query.Where(filter);
        }

        return await query.ToPagedTupleAsync(page, pageSize);
    }

    public async Task<(List<Tournament> Items, int TotalCount)> GetFilteredPagedAsync(
        TournamentListQuery query,
        int page,
        int pageSize)
    {
        var tournamentsQuery = _context.Tournaments
            .Include(t => t.Applications)
            .Include(t => t.Photos)
            .Include(t => t.Track)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            tournamentsQuery = tournamentsQuery.Where(t =>
                t.Name.Contains(query.Search) ||
                t.Description.Contains(query.Search));
        }

        if (query.Status.HasValue)
        {
            tournamentsQuery = tournamentsQuery.Where(t => t.Status == query.Status.Value);
        }

        if (query.TrackId.HasValue)
        {
            tournamentsQuery = tournamentsQuery.Where(t => t.TrackId == query.TrackId.Value);
        }

        return await tournamentsQuery.ToPagedTupleAsync(page, pageSize);
    }

    public async Task<List<Tournament>> GetByStatusAsync(TournamentStatus status)
    {
        return await _context.Tournaments
            .Where(t => t.Status == status)
            .Include(t => t.Applications)
            .Include(t => t.Photos)
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
            .Include(a => a.Car)
                .ThenInclude(c => c.Photos)
            .Include(a => a.User)
                .ThenInclude(u => u.Avatar)
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

    public async Task ApproveApplicationAtomicallyAsync(Guid applicationId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.RepeatableRead);

        try
        {
            var application = await _context.TournamentApplications.FindAsync(applicationId);
            if (application == null || application.Status != TournamentApplicationStatus.Pending)
            {
                return;
            }

            application.Status = TournamentApplicationStatus.Approved;
            _ = _context.TournamentApplications.Update(application);
            _ = await _context.SaveChangesAsync();

            var tournament = await _context.Tournaments.FindAsync(application.TournamentId);
            if (tournament != null && tournament.Status == TournamentStatus.RegistrationOpen)
            {
                var approvedCount = await _context.TournamentApplications
                    .CountAsync(a => a.TournamentId == tournament.Id && a.Status == TournamentApplicationStatus.Approved);

                if (approvedCount >= tournament.RequiredParticipants)
                {
                    tournament.Status = TournamentStatus.Confirmed;
                    _ = _context.Tournaments.Update(tournament);
                    _ = await _context.SaveChangesAsync();
                }
            }

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
