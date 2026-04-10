namespace MotorsportErp.Application.DTO.Common;

public class ReferenceDataResponse
{
    public List<EnumValueResponse> CarClasses { get; set; } = [];
    public List<EnumValueResponse> TrackStatuses { get; set; } = [];
    public List<EnumValueResponse> TournamentStatuses { get; set; } = [];
    public List<EnumValueResponse> TournamentApplicationStatuses { get; set; } = [];
    public List<EnumValueResponse> UserRoles { get; set; } = [];

    public int MinRacesToCreateTrack { get; set; }
    public int DefaultTrackConfirmationThreshold { get; set; }
    public int MinRacesToBecomeOrganizer { get; set; }
}
