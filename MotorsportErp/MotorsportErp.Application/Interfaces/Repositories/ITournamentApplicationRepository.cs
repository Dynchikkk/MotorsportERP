using MotorsportErp.Domain.Tournaments;

namespace MotorsportErp.Application.Interfaces.Repositories;

public interface ITournamentApplicationRepository
{
    Task<TournamentApplication?> GetByIdAsync(Guid id);
    Task UpdateAsync(TournamentApplication application);
}
