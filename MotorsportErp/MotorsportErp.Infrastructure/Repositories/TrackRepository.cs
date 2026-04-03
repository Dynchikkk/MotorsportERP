using Microsoft.EntityFrameworkCore;
using MotorsportErp.Application.Interfaces.Repositories;
using MotorsportErp.Domain.Tracks;
using MotorsportErp.Infrastructure.Extensions;
using MotorsportErp.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace MotorsportErp.Infrastructure.Repositories;

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
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<(List<Track> Items, int TotalCount)> GetPagedAsync(
            Expression<Func<Track, bool>>? filter,
            int page,
            int pageSize)
    {
        var query = _context.Tracks.AsQueryable();

        if (filter != null)
        {
            query = query.Where(filter);
        }

        return await query.ToPagedTupleAsync(page, pageSize);
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