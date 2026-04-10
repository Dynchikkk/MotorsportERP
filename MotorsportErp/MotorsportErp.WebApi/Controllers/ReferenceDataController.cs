using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MotorsportErp.Application.Common.Settings;
using MotorsportErp.Application.DTO.Common;
using MotorsportErp.Domain.Cars;
using MotorsportErp.Domain.Tournaments;
using MotorsportErp.Domain.Tracks;
using MotorsportErp.Domain.Users;
using System.Net.Mime;

namespace MotorsportErp.WebApi.Controllers;

[ApiController]
[Route("api/reference-data")]
[AllowAnonymous]
[Produces(MediaTypeNames.Application.Json)]
public class ReferenceDataController : ControllerBase
{
    private readonly TrackSettings _trackSettings;
    private readonly TournamentSettings _tournamentSettings;

    public ReferenceDataController(
        IOptions<TrackSettings> trackSettings,
        IOptions<TournamentSettings> tournamentSettings)
    {
        _trackSettings = trackSettings.Value;
        _tournamentSettings = tournamentSettings.Value;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ReferenceDataResponse), StatusCodes.Status200OK)]
    public ActionResult<ReferenceDataResponse> Get()
    {
        return Ok(new ReferenceDataResponse
        {
            CarClasses = MapEnum<CarClass>(),
            TrackStatuses = MapEnum<TrackStatus>(),
            TournamentStatuses = MapEnum<TournamentStatus>(),
            TournamentApplicationStatuses = MapEnum<TournamentApplicationStatus>(),
            UserRoles = MapEnum<UserRole>().Where(x => x.Value != (int)UserRole.None).ToList(),
            MinRacesToCreateTrack = _trackSettings.MinRacesToCreate,
            DefaultTrackConfirmationThreshold = _trackSettings.DefaultConfirmationThreshold,
            MinRacesToBecomeOrganizer = _tournamentSettings.MinRacesToBecomeOrganizer
        });
    }

    private static List<EnumValueResponse> MapEnum<TEnum>() where TEnum : struct, Enum
    {
        return Enum.GetValues<TEnum>()
            .Select(value => new EnumValueResponse
            {
                Value = Convert.ToInt32(value),
                Name = value.ToString()
            })
            .ToList();
    }
}
