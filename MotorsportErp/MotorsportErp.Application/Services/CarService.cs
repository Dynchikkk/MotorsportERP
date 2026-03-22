using MotorsportErp.Application.DTO.Cars;
using MotorsportErp.Application.Interfaces.Repositories;
using MotorsportErp.Application.Interfaces.Services;
using MotorsportErp.Application.Mappers;
using MotorsportErp.Domain.Tournaments;

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

    public async Task<List<CarResponse>> GetUserCarsAsync(Guid userId)
    {
        _ = await _userRepository.GetByIdAsync(userId) ?? throw new KeyNotFoundException("User not found");
        List<Domain.Cars.Car> cars = await _carRepository.GetByUserIdAsync(userId);
        return CarMapper.ToResponseList(cars).ToList();
    }

    public async Task<Guid> CreateAsync(Guid userId, CarCreateRequest request)
    {
        Domain.Users.User user = await _userRepository.GetByIdAsync(userId) ?? throw new KeyNotFoundException("User not found");

        Domain.Cars.Car car = CarMapper.ToEntity(request, user.Id);
        await _carRepository.AddAsync(car);

        return car.Id;
    }

    public async Task UpdateAsync(Guid userId, Guid carId, CarUpdateRequest request)
    {
        Domain.Cars.Car car = await _carRepository.GetByIdAsync(carId) ?? throw new KeyNotFoundException("Car not found");
        if (car.OwnerId != userId)
        {
            throw new UnauthorizedAccessException("You are not the owner of this car");
        }

        CarMapper.Update(car, request);

        await _carRepository.UpdateAsync(car);
    }

    public async Task DeleteAsync(Guid userId, Guid carId)
    {
        Domain.Cars.Car car = await _carRepository.GetByIdAsync(carId) ?? throw new KeyNotFoundException("Car not found");
        if (car.OwnerId != userId)
        {
            throw new UnauthorizedAccessException("You are not the owner of this car");
        }

        bool hasActiveApplications = car.Applications.Any(a =>
            a.Tournament.Status is TournamentStatus.RegistrationOpen or
            TournamentStatus.Confirmed or
            TournamentStatus.Active);

        if (hasActiveApplications)
        {
            throw new InvalidOperationException("Cannot delete car used in active tournaments");
        }

        await _carRepository.DeleteAsync(car);
    }
}