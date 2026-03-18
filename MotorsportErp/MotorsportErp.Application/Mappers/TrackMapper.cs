
using MotorsportErp.Application.DTO.Tracks;
using MotorsportErp.Domain.Tracks;

namespace MotorsportErp.Application.Mappers;
public static class TrackMapper
{
    public static TrackResponse ToResponse(Track track)
    {
        return new TrackResponse
        {
            Id = track.Id,
            Name = track.Name,
            Location = track.Location,
            LayoutImageUrl = track.LayoutImageUrl,
            Status = track.Status,
            VoteCount = track.VoteCount
        };
    }

    public static TrackDetailsResponse ToDetails(Track track)
    {
        return new TrackDetailsResponse
        {
            Id = track.Id,
            Name = track.Name,
            Location = track.Location,
            LayoutImageUrl = track.LayoutImageUrl,
            Status = track.Status,
            VoteCount = track.VoteCount,
            ConfirmationThreshold = track.ConfirmationThreshold
        };
    }

    public static IEnumerable<TrackResponse> ToResponseList(IEnumerable<Track> tracks)
    {
        return tracks.Select(ToResponse);
    }

    public static Track ToEntity(TrackCreateRequest request, Guid creatorId)
    {
        return new Track
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Location = request.Location,
            LayoutImageUrl = request.LayoutImageUrl,
            CreatedById = creatorId
        };
    }
}