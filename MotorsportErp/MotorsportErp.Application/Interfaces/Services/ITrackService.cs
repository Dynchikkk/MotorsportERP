using MotorsportErp.Application.DTO.Tracks;

namespace MotorsportErp.Application.Interfaces.Services;

public interface ITrackService
{
    Task<List<TrackResponse>> GetAllAsync();

    Task<TrackDetailsResponse> GetByIdAsync(Guid id);

    Task<Guid> CreateAsync(Guid userId, TrackCreateRequest request);

    Task VoteAsync(Guid userId, TrackVoteRequest request);

    Task UpdateAsync(Guid userId, Guid trackId, TrackUpdateRequest request);

    Task DeleteAsync(Guid userId, Guid trackId);

    Task ConfirmAsync(Guid userId, Guid trackId);

    Task MakeOfficialAsync(Guid userId, Guid trackId);
}