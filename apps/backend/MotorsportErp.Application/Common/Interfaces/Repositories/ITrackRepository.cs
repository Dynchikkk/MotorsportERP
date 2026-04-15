using MotorsportErp.Application.Features.Tracks.Contracts;
using MotorsportErp.Domain.Tracks;

namespace MotorsportErp.Application.Common.Interfaces.Repositories;

public interface ITrackRepository : IBaseRepository<Track>, IPagedRepository<Track>
{
    Task<(List<Track> Items, int TotalCount)> GetFilteredPagedAsync(
        TrackListQuery query,
        int page,
        int pageSize);

    Task<bool> HasUserVotedAsync(Guid trackId, Guid userId);

    Task AddVoteAsync(TrackVote vote);
}
