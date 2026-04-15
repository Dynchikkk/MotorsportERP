namespace MotorsportErp.Application.Common.Interfaces.Services;

public interface IPhotoGalleryService
{
    Task AddPhotoAsync(Guid userId, Guid targetEntityId, Guid photoId);
    Task RemovePhotoAsync(Guid userId, Guid targetEntityId, Guid photoId);
}
