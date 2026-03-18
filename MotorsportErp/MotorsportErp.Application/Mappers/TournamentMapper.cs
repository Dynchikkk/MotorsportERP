
using MotorsportErp.Application.DTO.Tournaments;
using MotorsportErp.Domain.Tournaments;

namespace MotorsportErp.Application.Mappers;
public static class TournamentMapper
{
    public static TournamentResponse ToResponse(Tournament tournament)
    {
        return new TournamentResponse
        {
            Id = tournament.Id,
            Name = tournament.Name,
            Status = tournament.Status,
            StartDate = tournament.StartDate,
            EndDate = tournament.EndDate,
            TrackId = tournament.TrackId,
            ParticipantsCount = tournament.Applications.Count
        };
    }

    public static TournamentDetailsResponse ToDetails(Tournament tournament)
    {
        return new TournamentDetailsResponse
        {
            Id = tournament.Id,
            Name = tournament.Name,
            Status = tournament.Status,
            StartDate = tournament.StartDate,
            EndDate = tournament.EndDate,
            TrackId = tournament.TrackId,
            Description = tournament.Description,
            ParticipantsCount = tournament.Applications.Count,
            RequiredParticipants = tournament.RequiredParticipants
        };
    }

    public static IEnumerable<TournamentResponse> ToResponseList(IEnumerable<Tournament> tournaments)
    {
        return tournaments.Select(ToResponse);
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

    public static void Update(Tournament tournament, TournamentUpdateRequest request)
    {
        tournament.Description = request.Description;
        tournament.StartDate = request.StartDate;
        tournament.EndDate = request.EndDate;
        tournament.RequiredParticipants = request.RequiredParticipants;
    }
}