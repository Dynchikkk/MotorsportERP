using MotorsportErp.Domain.Cars;

namespace MotorsportErp.Application.Interfaces.Repositories;

public interface ICarRepository : IBaseRepository<Car>
{
    Task<List<Car>> GetByUserIdAsync(Guid ownerId);

    Task<bool> HasActiveApplicationsAsync(Guid carId);
}