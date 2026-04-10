using MotorsportErp.Application.DTO.Common;
using MotorsportErp.Application.DTO.Tournaments;

namespace MotorsportErp.Application.Interfaces.Services;

public interface ITournamentService : IPhotoService
{
    Task<PagedResponse<TournamentResponse>> GetAllAsync(
        TournamentListQuery query,
        int page = 0,
        int pageSize = 20);
    Task<TournamentDetailsResponse> GetByIdAsync(Guid id);
    Task<IReadOnlyCollection<TournamentApplicationResponse>> GetApplicationsAsync(Guid userId, Guid tournamentId);

    Task<Guid> CreateAsync(Guid userId, TournamentCreateRequest request);
    Task UpdateAsync(Guid userId, Guid tournamentId, TournamentUpdateRequest request);
    Task DeleteAsync(Guid userId, Guid tournamentId);

    Task AddOrganizerAsync(Guid userId, Guid tournamentId, Guid newOrganizerId);

    Task StartAsync(Guid userId, Guid tournamentId);
    Task FinishAsync(Guid userId, Guid tournamentId);
    Task CancelAsync(Guid userId, Guid tournamentId);
    Task AddResultAsync(Guid userId, Guid tournamentId, TournamentResultRequest request);

    Task ApplyAsync(Guid userId, Guid tournamentId, Guid carId);
    Task CancelApplicationAsync(Guid userId, Guid applicationId);
    Task ApproveAsync(Guid userId, Guid applicationId);
    Task RejectAsync(Guid userId, Guid applicationId);
}
