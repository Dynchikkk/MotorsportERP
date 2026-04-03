using MotorsportErp.Domain.Tracks;

namespace MotorsportErp.Application.Interfaces.Repositories;

public interface ITrackRepository : IBaseRepository<Track>, IPagedRepository<Track>
{
    Task<bool> HasUserVotedAsync(Guid trackId, Guid userId);

    Task AddVoteAsync(TrackVote vote);
}