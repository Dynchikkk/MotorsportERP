using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotorsportErp.Application.DTO.Common;
using MotorsportErp.Application.DTO.Tournaments;
using MotorsportErp.Application.Interfaces.Services;
using MotorsportErp.WebApi.Extensions;
using System.Net.Mime;

namespace MotorsportErp.WebApi.Controllers;

/// <summary>
/// Controller responsible for managing tournaments, including creation, 
/// status lifecycle, applying for participation, and submitting results.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize] // Requires authentication by default for all endpoints
[Produces(MediaTypeNames.Application.Json)]
public class TournamentsController : ControllerBase
{
    private readonly ITournamentService _tournamentService;

    public TournamentsController(ITournamentService tournamentService)
    {
        _tournamentService = tournamentService;
    }

    /// <summary>
    /// Retrieves a paginated list of all tournaments (Publicly accessible).
    /// </summary>
    /// <param name="page">The page number (0-indexed).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A paginated list of TournamentResponse DTOs.</returns>
    [HttpGet]
    [AllowAnonymous] // Anyone can view the list of tournaments
    [ProducesResponseType(typeof(PagedResponse<TournamentResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<TournamentResponse>>> GetAll(
        [FromQuery] int page = 0,
        [FromQuery] int pageSize = 20)
    {
        var result = await _tournamentService.GetAllAsync(page, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves detailed information about a specific tournament (Publicly accessible).
    /// </summary>
    /// <param name="id">The unique identifier of the tournament.</param>
    /// <returns>Detailed tournament information including settings and participants count.</returns>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TournamentDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TournamentDetailsResponse>> GetById(Guid id)
    {
        var result = await _tournamentService.GetByIdAsync(id);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new tournament. The creator automatically becomes an organizer.
    /// User must meet the required race count or possess privileged roles.
    /// </summary>
    /// <param name="request">The tournament creation payload.</param>
    /// <returns>The ID of the newly created tournament.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Guid>> Create([FromBody] TournamentCreateRequest request)
    {
        var userId = User.GetUserId();
        var tournamentId = await _tournamentService.CreateAsync(userId, request);

        // Return 201 Created with the generated ID
        return StatusCode(StatusCodes.Status201Created, tournamentId);
    }

    /// <summary>
    /// Updates the mutable parameters of a tournament (description, dates, required participants).
    /// Allowed only before registration closes and only for organizers/moderators.
    /// </summary>
    /// <param name="id">The ID of the tournament to update.</param>
    /// <param name="request">The update payload.</param>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] TournamentUpdateRequest request)
    {
        var userId = User.GetUserId();
        await _tournamentService.UpdateAsync(userId, id, request);
        return NoContent();
    }

    /// <summary>
    /// Deletes a tournament completely from the system.
    /// Restricted to Moderators and SuperAdmins only.
    /// </summary>
    /// <param name="id">The ID of the tournament to delete.</param>
    [HttpDelete("{id}")]
    [Authorize(Policy = "RequireModerator")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = User.GetUserId();
        await _tournamentService.DeleteAsync(userId, id);
        return NoContent();
    }

    /// <summary>
    /// Assigns a new user as a co-organizer for the specified tournament.
    /// </summary>
    /// <param name="id">The tournament ID.</param>
    /// <param name="newOrganizerId">The user ID of the new organizer.</param>
    [HttpPost("{id}/organizers/{newOrganizerId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddOrganizer(Guid id, Guid newOrganizerId)
    {
        var userId = User.GetUserId();
        await _tournamentService.AddOrganizerAsync(userId, id, newOrganizerId);
        return NoContent();
    }

    /// <summary>
    /// Changes the tournament status to Active. 
    /// Allowed only if the tournament is confirmed and the start date has passed.
    /// </summary>
    /// <param name="id">The tournament ID.</param>
    [HttpPost("{id}/start")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Start(Guid id)
    {
        var userId = User.GetUserId();
        await _tournamentService.StartAsync(userId, id);
        return NoContent();
    }

    /// <summary>
    /// Finishes the tournament. Requires results to be inputted beforehand.
    /// Automatically increments the race count for all participating users.
    /// </summary>
    /// <param name="id">The tournament ID.</param>
    [HttpPost("{id}/finish")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Finish(Guid id)
    {
        var userId = User.GetUserId();
        await _tournamentService.FinishAsync(userId, id);
        return NoContent();
    }

    /// <summary>
    /// Cancels the tournament. Cannot be done if the tournament is already finished.
    /// </summary>
    /// <param name="id">The tournament ID.</param>
    [HttpPost("{id}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var userId = User.GetUserId();
        await _tournamentService.CancelAsync(userId, id);
        return NoContent();
    }

    /// <summary>
    /// Submits the race result (position, lap time) for a specific participant.
    /// Can only be done while the tournament is Active.
    /// </summary>
    /// <param name="id">The tournament ID.</param>
    /// <param name="request">The racer's result data.</param>
    [HttpPost("{id}/results")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AddResult(Guid id, [FromBody] TournamentResultRequest request)
    {
        var userId = User.GetUserId();
        await _tournamentService.AddResultAsync(userId, id, request);
        return NoContent();
    }

    /// <summary>
    /// Submits an application for the current user to participate in the tournament.
    /// Checks car class, user experience (race count), and tournament status.
    /// </summary>
    /// <param name="id">The tournament ID from the route.</param>
    /// <param name="request">Contains the Car ID chosen by the user.</param>
    [HttpPost("{id}/apply")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Apply(Guid id, [FromBody] TournamentApplyRequest request)
    {
        var userId = User.GetUserId();
        // ID from route overrides request body ID to prevent parameter tampering
        await _tournamentService.ApplyAsync(userId, id, request.CarId);
        return NoContent();
    }

    /// <summary>
    /// Add photo to gallery
    /// </summary>
    [HttpPost("{id}/photos/{photoId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AddPhoto(Guid id, Guid photoId)
    {
        await _tournamentService.AddPhotoAsync(User.GetUserId(), id, photoId);
        return NoContent();
    }

    /// <summary>
    /// Remove photo from gallery
    /// </summary>
    [HttpDelete("{id}/photos/{photoId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RemovePhoto(Guid id, Guid photoId)
    {
        await _tournamentService.RemovePhotoAsync(User.GetUserId(), id, photoId);
        return NoContent();
    }
}