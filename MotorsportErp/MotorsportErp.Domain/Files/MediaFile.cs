using MotorsportErp.Domain.BaseEntities;
using MotorsportErp.Domain.Cars;
using MotorsportErp.Domain.Tournaments;
using MotorsportErp.Domain.Tracks;
using MotorsportErp.Domain.Users;

namespace MotorsportErp.Domain.Files;

public class MediaFile : GuidEntity
{
    public required string OriginalFileName { get; set; }
    public required string SavedUrl { get; set; }

    // Navigation

    public Guid? UploadedById { get; set; }
    public User? UploadedBy { get; set; }

    public ICollection<Car> Cars { get; set; } = [];
    public ICollection<Track> Tracks { get; set; } = [];
    public ICollection<Tournament> Tournaments { get; set; } = [];
}