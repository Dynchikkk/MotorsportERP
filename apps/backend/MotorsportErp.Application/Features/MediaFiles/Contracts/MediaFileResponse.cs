namespace MotorsportErp.Application.Features.MediaFiles.Contracts;

public class MediaFileResponse
{
    public Guid Id { get; set; }
    public string Url { get; set; } = default!;
}