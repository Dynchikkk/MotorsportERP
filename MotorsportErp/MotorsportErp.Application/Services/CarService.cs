using MotorsportErp.Application.DTO.Cars;
using MotorsportErp.Application.Interfaces.Repositories;
using MotorsportErp.Application.Interfaces.Services;
using MotorsportErp.Application.Mappers;
using MotorsportErp.Domain.Cars;

namespace MotorsportErp.Application.Services;

public class CarService : ICarService
{
    private readonly ICarRepository _carRepository;
    private readonly IUserRepository _userRepository;

    public CarService(
        ICarRepository carRepository,
        IUserRepository userRepository)
    {
        _carRepository = carRepository;
        _userRepository = userRepository;
    }

    public async Task<Guid> CreateAsync(Guid userId, CarCreateRequest request)
    {
        _ = await _userRepository.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found");

        var car = new Car
        {
            Id = Guid.NewGuid(),
            Model = request.Model,
            Brand = request.Brand,
            Year = request.Year,
            Description = request.Description,
            CarClass = request.CarClass,
            OwnerId = userId
        };

        await _carRepository.AddAsync(car);

        return car.Id;
    }

    public async Task<List<CarResponse>> GetUserCarsAsync(Guid userId)
    {
        var cars = await _carRepository.GetByUserIdAsync(userId);

        return cars.Select(CarMapper.ToResponse).ToList();
    }

    public async Task UpdateAsync(Guid userId, Guid carId, CarUpdateRequest request)
    {
        var car = await _carRepository.GetByIdAsync(carId)
            ?? throw new KeyNotFoundException("Car not found");

        if (car.OwnerId != userId)
        {
            throw new UnauthorizedAccessException("No permission");
        }

        car.Model = request.Model;
        car.Brand = request.Brand;
        car.Year = request.Year;
        car.Description = request.Description;
        car.CarClass = request.CarClass;
        car.OwnerId = userId;

        await _carRepository.UpdateAsync(car);
    }

    public async Task DeleteAsync(Guid userId, Guid carId)
    {
        var car = await _carRepository.GetByIdAsync(carId) ?? throw new KeyNotFoundException("Car not found");

        if (car.OwnerId != userId)
            throw new UnauthorizedAccessException("No permission");

        bool hasActive = await _carRepository.HasActiveApplicationsAsync(carId);
        if (hasActive)
            throw new InvalidOperationException("Cannot delete car used in active tournaments");

        await _carRepository.DeleteAsync(car);
    }
}