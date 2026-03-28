using MotorsportErp.Application.DTO.Tournaments;

namespace MotorsportErp.Application.Interfaces.Services;

public interface ITournamentService
{
    Task<Guid> CreateAsync(Guid userId, TournamentCreateRequest request);

    Task UpdateAsync(Guid userId, Guid tournamentId, TournamentUpdateRequest request);

    Task CancelAsync(Guid userId, Guid tournamentId);

    Task ApplyAsync(Guid userId, Guid tournamentId, Guid carId);

    Task ApproveAsync(Guid userId, Guid applicationId);
    Task RejectAsync(Guid userId, Guid applicationId);

    Task StartAsync(Guid userId, Guid tournamentId);
    Task FinishAsync(Guid userId, Guid tournamentId);

    Task AddResultAsync(Guid userId, Guid tournamentId, TournamentResultRequest request);

    Task<List<TournamentResponse>> GetAllAsync();
    Task<TournamentDetailsResponse> GetByIdAsync(Guid id);
}