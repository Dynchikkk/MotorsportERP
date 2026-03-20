using MotorsportErp.Domain.Tournaments;

namespace MotorsportErp.Application.Interfaces.Repositories;

public interface ITournamentRepository : IBaseRepository<Tournament>
{
    Task<List<Tournament>> GetAllAsync();

    Task<List<Tournament>> GetByStatusAsync(TournamentStatus status);

    Task AddApplicationAsync(TournamentApplication application);

    Task<TournamentApplication?> GetApplicationByIdAsync(Guid id);

    Task<List<TournamentApplication>> GetApplicationsByTournamentIdAsync(Guid tournamentId);

    Task AddResultAsync(TournamentResult result);

    Task<bool> HasUserAppliedAsync(Guid tournamentId, Guid userId);
}