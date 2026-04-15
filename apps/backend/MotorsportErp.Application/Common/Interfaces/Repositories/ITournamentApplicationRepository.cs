using MotorsportErp.Application.Common.Interfaces.Repositories;
using MotorsportErp.Domain.Tournaments;

public interface ITournamentApplicationRepository : IBaseRepository<TournamentApplication>
{
    Task<bool> ExistsAsync(Guid userId, Guid tournamentId);

    Task<int> GetApprovedCountAsync(Guid tournamentId);
}