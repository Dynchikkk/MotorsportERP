using MotorsportErp.Domain.Users;

namespace MotorsportErp.Application.Common.Interfaces.Repositories;

public interface IUserRepository : IBaseRepository<User>, IPagedRepository<User>
{
    Task<User?> GetByEmailAsync(string email);

    Task<bool> ExistsByEmailAsync(string email);
}