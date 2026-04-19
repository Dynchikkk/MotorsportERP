using MotorsportErp.Application.Common.Contracts;

namespace MotorsportErp.Application.Features.Tracks.Contracts;
public class TrackReferenceDataResponse
{
    public int MinRacesToCreateTrack { get; set; }
    public int DefaultTrackConfirmationThreshold { get; set; }

    public List<EnumValueResponse> TrackStatuses { get; set; } = [];
}
