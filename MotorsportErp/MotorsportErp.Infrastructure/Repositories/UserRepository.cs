using Microsoft.EntityFrameworkCore;
using MotorsportErp.Application.Interfaces.Repositories;
using MotorsportErp.Domain.Users;
using MotorsportErp.Infrastructure.Extensions;
using MotorsportErp.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace MotorsportErp.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users
            .Include(u => u.Cars)
            .Include(u => u.Applications)
            .Include(u => u.Results)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<(List<User> Items, int TotalCount)> GetPagedAsync(
            Expression<Func<User, bool>>? filter,
            int page,
            int pageSize)
    {
        var query = _context.Users.AsQueryable();

        if (filter != null)
        {
            query = query.Where(filter);
        }

        return await query.ToPagedTupleAsync(page, pageSize);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }

    public async Task AddAsync(User entity)
    {
        _ = await _context.Users.AddAsync(entity);
        _ = await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User entity)
    {
        _ = _context.Users.Update(entity);
        _ = await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(User entity)
    {
        _ = _context.Users.Remove(entity);
        _ = await _context.SaveChangesAsync();
    }
}