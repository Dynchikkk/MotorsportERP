using MotorsportErp.Application.Features.Cars.Contracts;
using MotorsportErp.Application.Features.Users.Contracts;
using MotorsportErp.Domain.Tournaments;

namespace MotorsportErp.Application.Features.Tournaments.Contracts;

public class TournamentApplicationResponse
{
    public Guid Id { get; set; }
    public TournamentApplicationStatus Status { get; set; }
    public UserResponse User { get; set; } = default!;
    public CarResponse Car { get; set; } = default!;
}
