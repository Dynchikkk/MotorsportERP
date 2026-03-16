using MotorsportErp.Domain.Cars;

namespace MotorsportErp.Application.DTO.Cars;

public class CarCreateRequest
{
    public CarClass CarClass { get; set; }
    public required string Brand { get; set; }
    public required string Model { get; set; }
    public int Year { get; set; }

    public string? Description { get; set; }
}