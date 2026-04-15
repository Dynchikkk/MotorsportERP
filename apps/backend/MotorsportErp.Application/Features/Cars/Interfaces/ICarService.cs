using MotorsportErp.Application.Common.Contracts;
using MotorsportErp.Application.Common.Interfaces.Services;
using MotorsportErp.Application.Features.Cars.Contracts;
using MotorsportErp.Application.Features.Tournaments.Contracts;

namespace MotorsportErp.Application.Features.Cars.Interfaces;

public interface ICarService : IPhotoGalleryService, IReferenceDataServices<CarReferenceDataResponce>
{
    Task<PagedResponse<CarResponse>> GetUserCarsAsync(Guid userId, int page = 0, int pageSize = 20);

    Task<Guid> CreateAsync(Guid userId, CarCreateRequest request);

    Task UpdateAsync(Guid userId, Guid carId, CarUpdateRequest request);

    Task DeleteAsync(Guid userId, Guid carId);
}