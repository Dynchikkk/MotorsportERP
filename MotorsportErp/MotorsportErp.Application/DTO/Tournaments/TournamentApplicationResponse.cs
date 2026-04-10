using MotorsportErp.Application.DTO.Cars;
using MotorsportErp.Application.DTO.Users;
using MotorsportErp.Domain.Tournaments;

namespace MotorsportErp.Application.DTO.Tournaments;

public class TournamentApplicationResponse
{
    public Guid Id { get; set; }
    public TournamentApplicationStatus Status { get; set; }
    public UserResponse User { get; set; } = default!;
    public CarResponse Car { get; set; } = default!;
}
