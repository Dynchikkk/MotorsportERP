using MotorsportErp.Application.DTO.Files;
using MotorsportErp.Application.DTO.Tournaments;
using MotorsportErp.Application.DTO.Users;
using MotorsportErp.Domain.Tracks;

namespace MotorsportErp.Application.DTO.Tracks;

public class TrackDetailsResponse
{
    public Guid Id { get; set; }

    public TrackStatus Status { get; set; }
    public string Name { get; set; } = default!;
    public string Location { get; set; } = default!;
    public List<MediaFileDto> Photos { get; set; } = [];

    public int VoteCount { get; set; }
    public int ConfirmationThreshold { get; set; }
    public UserResponse? CreatedBy { get; set; }
    public List<TournamentResponse> UpcomingTournaments { get; set; } = [];
    public List<TournamentResponse> PastTournaments { get; set; } = [];
}
