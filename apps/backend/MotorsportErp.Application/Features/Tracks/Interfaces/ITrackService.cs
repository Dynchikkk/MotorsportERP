using MotorsportErp.Application.Common.Contracts;
using MotorsportErp.Application.Common.Interfaces.Services;
using MotorsportErp.Application.Features.Tracks.Contracts;

namespace MotorsportErp.Application.Features.Tracks.Interfaces;

public interface ITrackService : IPhotoGalleryService, IReferenceDataServices<TrackReferenceDataResponce>
{
    Task<PagedResponse<TrackResponse>> GetAllAsync(TrackListQuery query, int page = 0, int pageSize = 20);
    Task<TrackDetailsResponse> GetByIdAsync(Guid id);

    Task<Guid> CreateAsync(Guid userId, TrackCreateRequest request);
    Task UpdateAsync(Guid userId, Guid trackId, TrackUpdateRequest request);
    Task DeleteAsync(Guid userId, Guid trackId);

    Task VoteAsync(Guid userId, Guid trackId, bool isPositive);
    Task ConfirmAsync(Guid userId, Guid trackId);
    Task MakeOfficialAsync(Guid userId, Guid trackId);
}
