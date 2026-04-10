using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotorsportErp.Application.DTO.Cars;
using MotorsportErp.Application.DTO.Common;
using MotorsportErp.Application.Interfaces.Services;
using MotorsportErp.WebApi.Extensions;
using System.Net.Mime;

namespace MotorsportErp.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces(MediaTypeNames.Application.Json)]
public class CarsController : ControllerBase
{
    private readonly ICarService _carService;

    public CarsController(ICarService carService)
    {
        _carService = carService;
    }

    /// <summary>
    /// Retrieves a paginated list of cars belonging to the currently authenticated user (My Garage).
    /// </summary>
    [HttpGet("my")]
    [ProducesResponseType(typeof(PagedResponse<CarResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<CarResponse>>> GetMyCars(
        [FromQuery] int page = 0,
        [FromQuery] int pageSize = 20)
    {
        var userId = User.GetUserId();
        var cars = await _carService.GetUserCarsAsync(userId, page, pageSize);

        return Ok(cars);
    }

    /// <summary>
    /// Retrieves a paginated list of cars belonging to a specific user (Public Garage).
    /// </summary>
    [HttpGet("user/{userId}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PagedResponse<CarResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<CarResponse>>> GetUserCars(
        Guid userId,
        [FromQuery] int page = 0,
        [FromQuery] int pageSize = 20)
    {
        var cars = await _carService.GetUserCarsAsync(userId, page, pageSize);

        return Ok(cars);
    }

    /// <summary>
    /// Adds a new car to the current user's garage.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> CreateCar([FromBody] CarCreateRequest request)
    {
        var userId = User.GetUserId();
        var carId = await _carService.CreateAsync(userId, request);

        return StatusCode(StatusCodes.Status201Created, carId);
    }

    /// <summary>
    /// Updates information about a specific car owned by the current user.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCar(Guid id, [FromBody] CarUpdateRequest request)
    {
        var userId = User.GetUserId();
        await _carService.UpdateAsync(userId, id, request);

        return NoContent();
    }

    /// <summary>
    /// Deletes a specific car owned by the current user. 
    /// Car cannot be deleted if it is used in active tournaments.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCar(Guid id)
    {
        var userId = User.GetUserId();
        await _carService.DeleteAsync(userId, id);

        return NoContent();
    }

    /// <summary>
    /// Add photo to gallery
    /// </summary>
    [HttpPost("{id}/photos/{photoId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AddPhoto(Guid id, Guid photoId)
    {
        await _carService.AddPhotoAsync(User.GetUserId(), id, photoId);
        return NoContent();
    }

    /// <summary>
    /// Remove photo from gallery
    /// </summary>
    [HttpDelete("{id}/photos/{photoId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RemovePhoto(Guid id, Guid photoId)
    {
        await _carService.RemovePhotoAsync(User.GetUserId(), id, photoId);
        return NoContent();
    }
}