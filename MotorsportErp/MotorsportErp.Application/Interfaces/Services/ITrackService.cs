using MotorsportErp.Application.DTO.Tracks;

namespace MotorsportErp.Application.Interfaces.Services;

public interface ITrackService
{
    Task<List<TrackResponse>> GetAllAsync();

    Task<TrackDetailsResponse> GetByIdAsync(Guid id);

    Task<Guid> CreateAsync(Guid userId, TrackCreateRequest request);

    Task VoteAsync(Guid userId, TrackVoteRequest request);
}