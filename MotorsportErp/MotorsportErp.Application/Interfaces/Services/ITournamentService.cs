using MotorsportErp.Application.DTO.Tournaments;

namespace MotorsportErp.Application.Interfaces.Services;

public interface ITournamentService
{
    Task<List<TournamentResponse>> GetAllAsync();

    Task<TournamentDetailsResponse> GetByIdAsync(Guid id);

    Task<Guid> CreateAsync(Guid userId, TournamentCreateRequest request);

    Task UpdateAsync(Guid tournamentId, TournamentUpdateRequest request);

    Task ApplyAsync(Guid userId, TournamentApplyRequest request);

    Task ApproveApplicationAsync(Guid userId, Guid applicationId);

    Task RejectApplicationAsync(Guid userId, Guid applicationId);

    Task AddResultAsync(Guid userId, Guid tournamentId, TournamentResultCreateRequest request);

    Task StartTournamentAsync(Guid userId, Guid tournamentId);

    Task FinishTournamentAsync(Guid userId, Guid tournamentId);
}