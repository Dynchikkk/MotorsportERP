using MotorsportErp.Application.DTO.Cars;
using MotorsportErp.Application.DTO.Common;

namespace MotorsportErp.Application.Interfaces.Services;

public interface ICarService : IPhotoService
{
    Task<PagedResponse<CarResponse>> GetUserCarsAsync(Guid userId, int page = 0, int pageSize = 20);

    Task<Guid> CreateAsync(Guid userId, CarCreateRequest request);

    Task UpdateAsync(Guid userId, Guid carId, CarUpdateRequest request);

    Task DeleteAsync(Guid userId, Guid carId);
}