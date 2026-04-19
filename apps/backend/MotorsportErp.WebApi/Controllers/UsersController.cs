using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotorsportErp.Application.Common.Contracts;
using MotorsportErp.Application.Features.Users.Contracts;
using MotorsportErp.Application.Features.Users.Interfaces;
using MotorsportErp.Domain.Users;
using MotorsportErp.WebApi.Extensions;
using System.Net.Mime;

namespace MotorsportErp.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces(MediaTypeNames.Application.Json)]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Retrieves user reference data
    /// </summary>
    /// <returns>Tournament user data.</returns>
    [HttpGet("referenceData")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(UserReferenceDataResponse), StatusCodes.Status200OK)]
    public ActionResult<UserReferenceDataResponse> GetReferenceData()
    {
        var result = _userService.GetReferenceData();
        return Ok(result);
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
    /// Retrieves the public profile of a specific user including cars and tournament history.
    /// </summary>
    [HttpGet("{id}/profile")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PublicUserProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PublicUserProfileResponse>> GetPublicProfile(Guid id)
    {
        var profile = await _userService.GetPublicProfileAsync(id);
        return Ok(profile);
    }

    /// <summary>
    /// Retrieves short info about the current user.
    /// </summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserResponse>> GetMe()
    {
        var userId = User.GetUserId();
        var user = await _userService.GetByIdAsync(userId);
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
    /// Retrieves all users base info
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PagedResponse<UserResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<UserResponse>>> GetAll(
        [FromQuery] string? search,
        [FromQuery] int page = 0,
        [FromQuery] int pageSize = 20)
    {
        var result = await _userService.GetAllAsync(search, page, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves all users admin info
    /// </summary>
    [HttpGet("admin")]
    [Authorize(Policy = "RequireModerator")]
    [ProducesResponseType(typeof(PagedResponse<UserAdminResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<UserAdminResponse>>> GetAllAdmin(
    [FromQuery] int page = 0,
    [FromQuery] int pageSize = 20)
    {
        var result = await _userService.GetAllAdminAsync(page, pageSize);
        return Ok(result);
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
    [Authorize(Policy = "RequireSuperAdmin")]
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
    [Authorize(Policy = "RequireModerator")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> SetBlockStatus(Guid id, [FromQuery] bool isBlocked)
    {
        var adminId = User.GetUserId();
        await _userService.BlockUserAsync(adminId, id, isBlocked);
        return NoContent();
    }
}
