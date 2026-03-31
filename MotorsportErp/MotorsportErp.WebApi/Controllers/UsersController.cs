using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotorsportErp.Application.DTO.Users;
using MotorsportErp.Application.Interfaces.Services;
using MotorsportErp.Domain.Users;
using MotorsportErp.WebApi.Extensions;
using System.Net.Mime;

namespace MotorsportErp.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // All endpoints require authentication by default
[Produces(MediaTypeNames.Application.Json)]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Retrieves public information about a specific user.
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> GetById(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);
        return Ok(user);
    }

    /// <summary>
    /// Retrieves the detailed profile of the currently authenticated user.
    /// </summary>
    [HttpGet("profile")]
    [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserProfileResponse>> GetMyProfile()
    {
        var userId = User.GetUserId();
        var profile = await _userService.GetProfileAsync(userId);
        return Ok(profile);
    }

    /// <summary>
    /// Updates the profile data of the currently authenticated user.
    /// </summary>
    [HttpPut("profile")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateProfile([FromBody] UserUpdateRequest request)
    {
        var userId = User.GetUserId();
        await _userService.UpdateProfileAsync(userId, request);
        return NoContent();
    }

    /// <summary>
    /// Assigns a specific role to a user. Restricted to SuperAdmin.
    /// </summary>
    [HttpPost("{id}/assign-role")]
    [Authorize(Roles = nameof(UserRole.SuperAdmin))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignRole(Guid id, [FromBody] UserRole role)
    {
        var adminId = User.GetUserId();
        await _userService.AssignRoleAsync(adminId, id, role);
        return NoContent();
    }

    /// <summary>
    /// Blocks or unblocks a user. Restricted to Moderators and SuperAdmins.
    /// </summary>
    [HttpPost("{id}/block")]
    [Authorize(Roles = $"{nameof(UserRole.Moderator)},{nameof(UserRole.SuperAdmin)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> SetBlockStatus(Guid id, [FromQuery] bool isBlocked)
    {
        var adminId = User.GetUserId();
        await _userService.BlockUserAsync(adminId, id, isBlocked);
        return NoContent();
    }
}