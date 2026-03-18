using MotorsportErp.Application.DTO.Tournaments;

namespace MotorsportErp.Application.Interfaces.Services;

public interface ITournamentService
{
    Task<List<TournamentResponse>> GetAllAsync();

    Task<TournamentDetailsResponse> GetByIdAsync(Guid id);

    Task<Guid> CreateAsync(Guid userId, TournamentCreateRequest request);

    Task UpdateAsync(Guid tournamentId, TournamentUpdateRequest request);

    Task ApplyAsync(Guid userId, TournamentApplyRequest request);

    Task ApproveApplicationAsync(Guid applicationId);

    Task RejectApplicationAsync(Guid applicationId);

    Task AddResultAsync(Guid tournamentId, TournamentResultCreateRequest request);
}