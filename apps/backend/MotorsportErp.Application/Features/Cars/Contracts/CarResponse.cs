using MotorsportErp.Application.Features.MediaFiles.Contracts;
using MotorsportErp.Domain.Cars;

namespace MotorsportErp.Application.Features.Cars.Contracts;

public class CarResponse
{
    public Guid Id { get; set; }

    public CarClass CarClass { get; set; }
    public string Brand { get; set; } = default!;
    public string Model { get; set; } = default!;
    public int Year { get; set; }

    public string? Description { get; set; }

    public List<MediaFileResponse> Photos { get; set; } = [];
}