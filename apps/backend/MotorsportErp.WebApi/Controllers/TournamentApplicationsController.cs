using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotorsportErp.Application.Features.Cars.Contracts;
using MotorsportErp.Application.Features.Tournaments.Contracts;
using MotorsportErp.Application.Features.Tournaments.Interfaces;
using MotorsportErp.WebApi.Extensions;
using System.Net.Mime;

namespace MotorsportErp.WebApi.Controllers;

/// <summary>
/// Controller responsible for managing individual tournament applications.
/// Handles the lifecycle of an application: Approval, Rejection by organizers, 
/// and Cancellation by the applicant.
/// </summary>
[ApiController]
[Route("api/applications")]
[Authorize] // All actions strictly require an authenticated user
[Produces(MediaTypeNames.Application.Json)]
public class TournamentApplicationsController : ControllerBase
{
    private readonly ITournamentService _tournamentService;

    public TournamentApplicationsController(ITournamentService tournamentService)
    {
        _tournamentService = tournamentService;
    }

    /// <summary>
    /// Retrieves tournament reference data
    /// </summary>
    /// <returns>Tournament reference data.</returns>
    [HttpGet("referenceData")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TournamentReferenceDataResponce), StatusCodes.Status200OK)]
    public ActionResult<TournamentReferenceDataResponce> GetReferenceData()
    {
        var result = _tournamentService.GetReferenceData();
        return Ok(result);
    }

    /// <summary>
    /// Cancels a submitted tournament application. 
    /// This action can only be performed by the user who created the application (the racer)
    /// and only while the tournament's registration is still open.
    /// </summary>
    /// <param name="id">The unique identifier of the tournament application.</param>
    [HttpPost("{id}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelApplication(Guid id)
    {
        // Extract the ID of the user making the request from the JWT token
        var userId = User.GetUserId();

        // Delegate the business logic to the service
        await _tournamentService.CancelApplicationAsync(userId, id);

        return NoContent();
    }

    /// <summary>
    /// Approves a pending tournament application.
    /// This action can only be performed by the tournament organizer, moderator, or super admin.
    /// If the required number of participants is reached, the tournament status may automatically change.
    /// </summary>
    /// <param name="id">The unique identifier of the tournament application.</param>
    [HttpPost("{id}/approve")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Approve(Guid id)
    {
        // Extract the ID of the current user (expected to be an organizer/admin)
        var userId = User.GetUserId();

        await _tournamentService.ApproveAsync(userId, id);

        return NoContent();
    }

    /// <summary>
    /// Rejects a pending tournament application.
    /// This action can only be performed by the tournament organizer, moderator, or super admin.
    /// </summary>
    /// <param name="id">The unique identifier of the tournament application.</param>
    [HttpPost("{id}/reject")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Reject(Guid id)
    {
        // Extract the ID of the current user (expected to be an organizer/admin)
        var userId = User.GetUserId();

        await _tournamentService.RejectAsync(userId, id);

        return NoContent();
    }
}