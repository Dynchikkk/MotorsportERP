namespace MotorsportErp.Application.Interfaces.Services;

public interface IPhotoService
{
    Task AddPhotoAsync(Guid userId, Guid targetEntityId, Guid photoId);
    Task RemovePhotoAsync(Guid userId, Guid targetEntityId, Guid photoId);
}
