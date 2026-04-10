using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotorsportErp.Application.DTO.Common;
using MotorsportErp.Application.DTO.Tracks;
using MotorsportErp.Application.Interfaces.Services;
using MotorsportErp.WebApi.Extensions;
using System.Net.Mime;

namespace MotorsportErp.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces(MediaTypeNames.Application.Json)]
public class TracksController : ControllerBase
{
    private readonly ITrackService trackService;

    public TracksController(ITrackService trackService)
    {
        this.trackService = trackService;
    }

    /// <summary>
    /// Получить список трасс (публично)
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PagedResponse<TrackResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<TrackResponse>>> GetAll(
        [FromQuery] TrackListQuery query,
        [FromQuery] int page = 0,
        [FromQuery] int pageSize = 20)
    {
        var result = await trackService.GetAllAsync(query, page, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// Получить детали трассы
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TrackDetailsResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<TrackDetailsResponse>> GetById(Guid id)
    {
        var result = await trackService.GetByIdAsync(id);
        return Ok(result);
    }

    /// <summary>
    /// Создать трассу
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    public async Task<ActionResult<Guid>> Create([FromBody] TrackCreateRequest request)
    {
        var userId = User.GetUserId();
        var id = await trackService.CreateAsync(userId, request);

        return StatusCode(StatusCodes.Status201Created, id);
    }

    /// <summary>
    /// Проголосовать за трассу
    /// </summary>
    [HttpPost("{id}/vote")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Vote(Guid id, [FromQuery] bool isPositive)
    {
        var userId = User.GetUserId();
        await trackService.VoteAsync(userId, id, isPositive);

        return NoContent();
    }

    /// <summary>
    /// Обновить трассу (автор или модератор)
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(Guid id, [FromBody] TrackUpdateRequest request)
    {
        var userId = User.GetUserId();
        await trackService.UpdateAsync(userId, id, request);

        return NoContent();
    }

    /// <summary>
    /// Подтвердить трассу (модератор)
    /// </summary>
    [HttpPost("{id}/confirm")]
    [Authorize(Policy = "RequireModerator")]
    public async Task<IActionResult> Confirm(Guid id)
    {
        var userId = User.GetUserId();
        await trackService.ConfirmAsync(userId, id);

        return NoContent();
    }

    /// <summary>
    /// Сделать трассу официальной (модератор)
    /// </summary>
    [HttpPost("{id}/official")]
    [Authorize(Policy = "RequireModerator")]
    public async Task<IActionResult> MakeOfficial(Guid id)
    {
        var userId = User.GetUserId();
        await trackService.MakeOfficialAsync(userId, id);

        return NoContent();
    }

    /// <summary>
    /// Удалить трассу (автор или модератор)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = User.GetUserId();
        await trackService.DeleteAsync(userId, id);

        return NoContent();
    }

    /// Add photo to gallery
    /// </summary>
    [HttpPost("{id}/photos/{photoId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AddPhoto(Guid id, Guid photoId)
    {
        await trackService.AddPhotoAsync(User.GetUserId(), id, photoId);
        return NoContent();
    }

    /// <summary>
    /// Remove photo from gallery
    /// </summary>
    [HttpDelete("{id}/photos/{photoId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RemovePhoto(Guid id, Guid photoId)
    {
        await trackService.RemovePhotoAsync(User.GetUserId(), id, photoId);
        return NoContent();
    }
}
