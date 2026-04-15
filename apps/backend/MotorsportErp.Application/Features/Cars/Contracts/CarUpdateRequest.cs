using MotorsportErp.Domain.Cars;

namespace MotorsportErp.Application.Features.Cars.Contracts;

public class CarUpdateRequest
{
    public required string Brand { get; set; }

    public required string Model { get; set; }

    public int Year { get; set; }

    public string? Description { get; set; }

    public CarClass CarClass { get; set; }
}