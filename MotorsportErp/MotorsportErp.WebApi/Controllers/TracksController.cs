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
    private readonly ITrackService _service;

    public TracksController(ITrackService service)
    {
        _service = service;
    }

    /// <summary>
    /// Получить список трасс (публично)
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PagedResponse<TrackResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<TrackResponse>>> GetAll(
        [FromQuery] int page = 0,
        [FromQuery] int pageSize = 20)
    {
        var result = await _service.GetAllAsync(page, pageSize);
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
        var result = await _service.GetByIdAsync(id);
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
        var id = await _service.CreateAsync(userId, request);

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
        await _service.VoteAsync(userId, id, isPositive);

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
        await _service.UpdateAsync(userId, id, request);

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
        await _service.ConfirmAsync(userId, id);

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
        await _service.MakeOfficialAsync(userId, id);

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
        await _service.DeleteAsync(userId, id);

        return NoContent();
    }
}