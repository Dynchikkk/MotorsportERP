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
            Status = track.Status,
            VoteCount = track.VoteCount,
            Photos = MediaFileMapper.ToResponseList(track.Photos)
        };
    }

    public static TrackDetailsResponse ToDetails(Track track)
    {
        var now = DateTime.UtcNow;

        return new TrackDetailsResponse
        {
            Id = track.Id,
            Name = track.Name,
            Location = track.Location,
            Status = track.Status,
            VoteCount = track.VoteCount,
            ConfirmationThreshold = track.ConfirmationThreshold,
            CreatedBy = track.CreatedBy != null ? UserMapper.ToResponse(track.CreatedBy) : null,
            UpcomingTournaments = track.Tournaments
                .Where(t => t.EndDate >= now)
                .OrderBy(t => t.StartDate)
                .Select(t => TournamentMapper.ToResponse(t, track.Name))
                .ToList(),
            PastTournaments = track.Tournaments
                .Where(t => t.EndDate < now)
                .OrderByDescending(t => t.StartDate)
                .Select(t => TournamentMapper.ToResponse(t, track.Name))
                .ToList(),
            Photos = MediaFileMapper.ToResponseList(track.Photos)
        };
    }

    public static Track ToEntity(TrackCreateRequest request, Guid creatorId)
    {
        return new Track
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Location = request.Location,
            CreatedById = creatorId
        };
    }
}
