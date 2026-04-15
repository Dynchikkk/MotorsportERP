using MotorsportErp.Domain.Cars;

namespace MotorsportErp.Application.Common.Interfaces.Repositories;

public interface ICarRepository : IBaseRepository<Car>, IPagedRepository<Car>
{
    Task<bool> HasActiveApplicationsAsync(Guid carId);
}