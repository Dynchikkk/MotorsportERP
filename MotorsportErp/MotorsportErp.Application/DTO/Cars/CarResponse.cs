using MotorsportErp.Application.DTO.Files;
using MotorsportErp.Domain.Cars;

namespace MotorsportErp.Application.DTO.Cars;

public class CarResponse
{
    public Guid Id { get; set; }

    public CarClass CarClass { get; set; }
    public string Brand { get; set; } = default!;
    public string Model { get; set; } = default!;
    public int Year { get; set; }

    public string? Description { get; set; }

    public List<MediaFileDto> Photos { get; set; } = [];
}