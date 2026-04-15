using MotorsportErp.Application.Common.Mappers;
using MotorsportErp.Application.Features.Cars.Mappers;
using MotorsportErp.Application.Features.Tournaments.Contracts;
using MotorsportErp.Application.Features.Tracks.Mappers;
using MotorsportErp.Application.Features.Users.Mappers;
using MotorsportErp.Domain.Tournaments;

namespace MotorsportErp.Application.Features.Tournaments.Mappers;
public static class TournamentMapper
{
    public static TournamentResponse ToResponse(Tournament tournament, string? trackNameOverride = null)
    {
        var approvedApplications = tournament.Applications.Count(a => a.Status == TournamentApplicationStatus.Approved);

        return new TournamentResponse
        {
            Id = tournament.Id,
            Name = tournament.Name,
            Status = tournament.Status,
            StartDate = tournament.StartDate,
            EndDate = tournament.EndDate,
            TrackId = tournament.TrackId,
            TrackName = trackNameOverride ?? tournament.Track?.Name ?? string.Empty,
            AllowedCarClass = tournament.AllowedCarClass,
            ParticipantsCount = approvedApplications,
            ApplicationsCount = tournament.Applications.Count,
            Photos = MediaFileMapper.ToResponseList(tournament.Photos)
        };
    }

    public static TournamentDetailsResponse ToDetails(Tournament tournament)
    {
        var approvedApplications = tournament.Applications
            .Where(a => a.Status == TournamentApplicationStatus.Approved)
            .ToList();

        return new TournamentDetailsResponse
        {
            Id = tournament.Id,
            Name = tournament.Name,
            Status = tournament.Status,
            StartDate = tournament.StartDate,
            EndDate = tournament.EndDate,
            TrackId = tournament.TrackId,
            Description = tournament.Description,
            AllowedCarClass = tournament.AllowedCarClass,
            RequiredRaceCount = tournament.RequiredRaceCount,
            ParticipantsCount = approvedApplications.Count,
            ApplicationsCount = tournament.Applications.Count,
            RequiredParticipants = tournament.RequiredParticipants,
            Track = tournament.Track != null ? TrackMapper.ToResponse(tournament.Track) : null,
            Organizers = tournament.Organizers
                .Where(o => o.User != null)
                .Select(o => UserMapper.ToResponse(o.User))
                .ToList(),
            Participants = approvedApplications
                .Select(ToApplicationResponse)
                .ToList(),
            Results = tournament.Results
                .OrderBy(r => r.Position)
                .Select(ToResultResponse)
                .ToList(),
            Photos = MediaFileMapper.ToResponseList(tournament.Photos)
        };
    }

    public static IEnumerable<TournamentResponse> ToResponseList(IEnumerable<Tournament> tournaments)
    {
        return tournaments.Select(t => ToResponse(t));
    }

    public static Tournament ToEntity(TournamentCreateRequest request, Guid creatorId)
    {
        return new Tournament
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            TrackId = request.TrackId,
            AllowedCarClass = request.AllowedCarClass,
            RequiredParticipants = request.RequiredParticipants,
            RequiredRaceCount = request.RequiredRaceCount,
            CreatorId = creatorId
        };
    }

    public static TournamentApplicationResponse ToApplicationResponse(TournamentApplication application)
    {
        return new TournamentApplicationResponse
        {
            Id = application.Id,
            Status = application.Status,
            User = UserMapper.ToResponse(application.User),
            Car = CarMapper.ToResponse(application.Car)
        };
    }

    public static TournamentResultResponse ToResultResponse(TournamentResult result)
    {
        return new TournamentResultResponse
        {
            Id = result.Id,
            Position = result.Position,
            BestLapTime = result.BestLapTime,
            User = UserMapper.ToResponse(result.User)
        };
    }
}
