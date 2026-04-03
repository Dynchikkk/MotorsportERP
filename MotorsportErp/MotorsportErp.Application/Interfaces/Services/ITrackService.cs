using MotorsportErp.Application.DTO.Common;
using MotorsportErp.Application.DTO.Tracks;

namespace MotorsportErp.Application.Interfaces.Services;

public interface ITrackService
{
    Task<PagedResponse<TrackResponse>> GetAllAsync(int page = 0, int pageSize = 20);
    Task<TrackDetailsResponse> GetByIdAsync(Guid id);

    Task<Guid> CreateAsync(Guid userId, TrackCreateRequest request);
    Task UpdateAsync(Guid userId, Guid trackId, TrackUpdateRequest request);
    Task DeleteAsync(Guid userId, Guid trackId);

    Task VoteAsync(Guid userId, Guid trackId, bool isPositive);
    Task ConfirmAsync(Guid userId, Guid trackId);
    Task MakeOfficialAsync(Guid userId, Guid trackId);
}