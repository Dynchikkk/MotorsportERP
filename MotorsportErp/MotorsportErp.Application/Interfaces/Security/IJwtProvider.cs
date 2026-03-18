using MotorsportErp.Domain.Users;

namespace MotorsportErp.Application.Interfaces.Security;

public interface IJwtProvider
{
    string GenerateToken(User user);
}