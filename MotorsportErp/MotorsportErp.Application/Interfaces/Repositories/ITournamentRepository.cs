using MotorsportErp.Application.DTO.Tournaments;
using MotorsportErp.Domain.Tournaments;

namespace MotorsportErp.Application.Interfaces.Repositories;

public interface ITournamentRepository : IBaseRepository<Tournament>, IPagedRepository<Tournament>
{
    Task<(List<Tournament> Items, int TotalCount)> GetFilteredPagedAsync(
        TournamentListQuery query,
        int page,
        int pageSize);

    Task<List<Tournament>> GetByStatusAsync(TournamentStatus status);

    Task AddApplicationAsync(TournamentApplication application);

    Task<TournamentApplication?> GetApplicationByIdAsync(Guid id);

    Task<List<TournamentApplication>> GetApplicationsByTournamentIdAsync(Guid tournamentId);

    Task AddResultAsync(TournamentResult result);

    Task ApproveApplicationAtomicallyAsync(Guid applicationId);

    Task<bool> HasUserAppliedAsync(Guid tournamentId, Guid userId);
}
