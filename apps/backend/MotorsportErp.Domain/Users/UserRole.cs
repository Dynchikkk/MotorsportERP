namespace MotorsportErp.Domain.Users;

[Flags]
public enum UserRole
{
    None = 0,
    Racer = 1,
    Organizer = 2,
    Moderator = 4,
    SuperAdmin = 8
}