using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotorsportErp.Application.Common.Contracts;
using MotorsportErp.Application.Features.Tracks.Contracts;
using MotorsportErp.Application.Features.Tracks.Interfaces;
using MotorsportErp.WebApi.Extensions;
using System.Net.Mime;

namespace MotorsportErp.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces(MediaTypeNames.Application.Json)]
public class TracksController : ControllerBase
{
    private readonly ITrackService _trackService;

    public TracksController(ITrackService trackService)
    {
        _trackService = trackService;
    }

    /// <summary>
    /// Retrieves track reference data
    /// </summary>
    /// <returns>Tournament track data.</returns>
    [HttpGet("referenceData")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TrackReferenceDataResponse), StatusCodes.Status200OK)]
    public ActionResult<TrackReferenceDataResponse> GetReferenceData()
    {
        var result = _trackService.GetReferenceData();
        return Ok(result);
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
        var result = await _trackService.GetAllAsync(query, page, pageSize);
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
        var result = await _trackService.GetByIdAsync(id);
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
        var id = await _trackService.CreateAsync(userId, request);

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
        await _trackService.VoteAsync(userId, id, isPositive);

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
        await _trackService.UpdateAsync(userId, id, request);

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
        await _trackService.ConfirmAsync(userId, id);

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
        await _trackService.MakeOfficialAsync(userId, id);

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
        await _trackService.DeleteAsync(userId, id);

        return NoContent();
    }

    /// Add photo to gallery
    /// </summary>
    [HttpPost("{id}/photos/{photoId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AddPhoto(Guid id, Guid photoId)
    {
        await _trackService.AddPhotoAsync(User.GetUserId(), id, photoId);
        return NoContent();
    }

    /// <summary>
    /// Remove photo from gallery
    /// </summary>
    [HttpDelete("{id}/photos/{photoId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RemovePhoto(Guid id, Guid photoId)
    {
        await _trackService.RemovePhotoAsync(User.GetUserId(), id, photoId);
        return NoContent();
    }
}
