
using MotorsportErp.Application.DTO.Cars;
using MotorsportErp.Domain.Cars;

namespace MotorsportErp.Application.Mappers;
public static class CarMapper
{
    public static CarResponse ToResponse(Car car)
    {
        return new CarResponse
        {
            Id = car.Id,
            Brand = car.Brand,
            Model = car.Model,
            Year = car.Year,
            Description = car.Description,
            CarClass = car.CarClass
        };
    }

    public static IEnumerable<CarResponse> ToResponseList(IEnumerable<Car> cars)
    {
        return cars.Select(ToResponse);
    }

    public static Car ToEntity(CarCreateRequest request, Guid ownerId)
    {
        return new Car
        {
            Id = Guid.NewGuid(),
            Brand = request.Brand,
            Model = request.Model,
            Year = request.Year,
            Description = request.Description,
            CarClass = request.CarClass,
            OwnerId = ownerId
        };
    }

    public static void Update(Car car, CarUpdateRequest request)
    {
        car.Brand = request.Brand;
        car.Model = request.Model;
        car.Year = request.Year;
        car.Description = request.Description;
        car.CarClass = request.CarClass;
    }
}