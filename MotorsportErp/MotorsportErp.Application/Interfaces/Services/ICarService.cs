using MotorsportErp.Application.DTO.Cars;

namespace MotorsportErp.Application.Interfaces.Services;

public interface ICarService
{
    Task<List<CarResponse>> GetUserCarsAsync(Guid userId);

    Task<Guid> CreateAsync(Guid userId, CarCreateRequest request);

    Task UpdateAsync(Guid carId, CarUpdateRequest request);

    Task DeleteAsync(Guid carId);
}