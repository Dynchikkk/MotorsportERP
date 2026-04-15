using Microsoft.EntityFrameworkCore;
using MotorsportErp.Application.Common.Interfaces.Repositories;
using MotorsportErp.Application.Features.Tracks.Contracts;
using MotorsportErp.Domain.Tracks;
using MotorsportErp.Infrastructure.Extensions;
using MotorsportErp.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace MotorsportErp.Infrastructure.Persistence.Repositories;

public class TrackRepository : ITrackRepository
{
    private readonly AppDbContext _context;

    public TrackRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Track?> GetByIdAsync(Guid id)
    {
        return await _context.Tracks
            .Include(t => t.Votes)
            .Include(t => t.Photos)
            .Include(t => t.CreatedBy)
                .ThenInclude(u => u.Avatar)
            .Include(t => t.Tournaments)
                .ThenInclude(tournament => tournament.Applications)
            .Include(t => t.Tournaments)
                .ThenInclude(tournament => tournament.Photos)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<(List<Track> Items, int TotalCount)> GetPagedAsync(
            Expression<Func<Track, bool>>? filter,
            int page,
            int pageSize)
    {
        var query = _context.Tracks
            .Include(t => t.Photos)
            .AsQueryable();

        if (filter != null)
        {
            query = query.Where(filter);
        }

        return await query.ToPagedTupleAsync(page, pageSize);
    }

    public async Task<(List<Track> Items, int TotalCount)> GetFilteredPagedAsync(
        TrackListQuery query,
        int page,
        int pageSize)
    {
        var tracksQuery = _context.Tracks
            .Include(t => t.Photos)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            tracksQuery = tracksQuery.Where(t =>
                t.Name.Contains(query.Search) ||
                t.Location.Contains(query.Search));
        }

        if (query.Status.HasValue)
        {
            tracksQuery = tracksQuery.Where(t => t.Status == query.Status.Value);
        }

        return await tracksQuery.ToPagedTupleAsync(page, pageSize);
    }

    public async Task<bool> HasUserVotedAsync(Guid trackId, Guid userId)
    {
        return await _context.TrackVotes
            .AnyAsync(v => v.TrackId == trackId && v.UserId == userId);
    }

    public async Task AddVoteAsync(TrackVote vote)
    {
        _ = await _context.TrackVotes.AddAsync(vote);
        _ = await _context.SaveChangesAsync();
    }

    public async Task AddAsync(Track entity)
    {
        _ = await _context.Tracks.AddAsync(entity);
        _ = await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Track entity)
    {
        _ = _context.Tracks.Update(entity);
        _ = await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Track entity)
    {
        _ = _context.Tracks.Remove(entity);
        _ = await _context.SaveChangesAsync();
    }
}
