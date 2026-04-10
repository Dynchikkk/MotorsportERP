using MotorsportErp.Application.Common.Exceptions;
using MotorsportErp.Application.DTO.Cars;
using MotorsportErp.Application.DTO.Common;
using MotorsportErp.Application.Interfaces.Repositories;
using MotorsportErp.Application.Interfaces.Services;
using MotorsportErp.Application.Mappers;
using MotorsportErp.Domain.Cars;
using MotorsportErp.Domain.Files;
using MotorsportErp.Domain.Users;

namespace MotorsportErp.Application.Services;

public class CarService : ICarService
{
    private readonly ICarRepository _carRepository;
    private readonly IUserRepository _userRepository;
    private readonly IFileRepository _fileRepository;

    public CarService(
        ICarRepository carRepository,
        IUserRepository userRepository,
        IFileRepository fileRepository)
    {
        _carRepository = carRepository;
        _userRepository = userRepository;
        _fileRepository = fileRepository;
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

    public async Task<PagedResponse<CarResponse>> GetUserCarsAsync(Guid userId, int page = 0, int pageSize = 20)
    {
        var (cars, totalCount) = await _carRepository.GetPagedAsync(c => c.OwnerId == userId, page, pageSize);

        return new PagedResponse<CarResponse>
        {
            Items = cars.Select(CarMapper.ToResponse).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task UpdateAsync(Guid userId, Guid carId, CarUpdateRequest request)
    {
        var car = await _carRepository.GetByIdAsync(carId) ?? throw new KeyNotFoundException("Car not found");
        if (car.OwnerId != userId)
        {
            throw new ForbiddenException();
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
        {
            throw new ForbiddenException();
        }

        bool hasActive = await _carRepository.HasActiveApplicationsAsync(carId);
        if (hasActive)
        {
            throw new InvalidOperationException("Cannot delete car used in active tournaments");
        }

        await _carRepository.DeleteAsync(car);
    }

    public async Task AddPhotoAsync(Guid userId, Guid targetEntityId, Guid photoId)
    {
        var car = await _carRepository.GetByIdAsync(targetEntityId) ?? throw new KeyNotFoundException("Car not found");
        if (car.OwnerId != userId)
        {
            throw new ForbiddenException();
        }

        var photo = await _fileRepository.GetByIdAsync(photoId) ?? throw new KeyNotFoundException("Photo not found");
        if (photo.UploadedById != userId)
        {
            throw new ForbiddenException("Only owner can use self photos");
        }

        car.Photos.Add(photo);
        await _carRepository.UpdateAsync(car);
    }

    public async Task RemovePhotoAsync(Guid userId, Guid targetEntityId, Guid photoId)
    {
        var car = await _carRepository.GetByIdAsync(targetEntityId) ?? throw new KeyNotFoundException();
        if (car.OwnerId != userId)
        {
            throw new ForbiddenException();
        }

        var photo = await _fileRepository.GetByIdAsync(photoId);
        if (photo != null)
        {
            _ = car.Photos.Remove(photo);
            await _carRepository.UpdateAsync(car);
        }
    }
}