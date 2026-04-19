using MotorsportErp.Application.Features.MediaFiles.Contracts;
using MotorsportErp.Application.Features.Tournaments.Contracts;
using MotorsportErp.Application.Features.Users.Contracts;
using MotorsportErp.Domain.Tracks;

namespace MotorsportErp.Application.Features.Tracks.Contracts;

public class TrackDetailsResponse
{
    public Guid Id { get; set; }

    public TrackStatus Status { get; set; }
    public string Name { get; set; } = default!;
    public string Location { get; set; } = default!;
    public List<MediaFileResponse> Photos { get; set; } = [];

    public int VoteCount { get; set; }
    public int ConfirmationThreshold { get; set; }
    public UserResponse? CreatedBy { get; set; }
    public List<TournamentResponse> UpcomingTournaments { get; set; } = [];
    public List<TournamentResponse> PastTournaments { get; set; } = [];
}
