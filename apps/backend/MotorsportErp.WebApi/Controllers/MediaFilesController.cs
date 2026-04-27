using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotorsportErp.Application.Features.MediaFiles.Interfaces;
using MotorsportErp.WebApi.Extensions;

namespace MotorsportErp.WebApi.Controllers;

[ApiController]
[Route("api/files")]
[Authorize]
public class MediaFilesController : ControllerBase
{
    private readonly IMediaFileService _fileService;

    public MediaFilesController(IMediaFileService fileService)
    {
        _fileService = fileService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("File is empty");
        }

        var userId = User.GetUserId();
        using var stream = file.OpenReadStream();
        var result = await _fileService.UploadImageAsync(stream, file.FileName, file.ContentType, file.Length, userId);

        return Created(result.Url, result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = User.GetUserId();
        await _fileService.DeleteFileAsync(id, userId);
        return NoContent();
    }
}
