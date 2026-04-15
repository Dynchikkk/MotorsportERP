using Microsoft.EntityFrameworkCore;
using MotorsportErp.Application.Common.Interfaces.Repositories;
using MotorsportErp.Domain.Users;
using MotorsportErp.Infrastructure.Extensions;
using MotorsportErp.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace MotorsportErp.Infrastructure.Persistence.Repositories;

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
                .ThenInclude(c => c.Photos)
            .Include(u => u.Applications)
                .ThenInclude(a => a.Tournament)
                    .ThenInclude(t => t.Track)
            .Include(u => u.Applications)
                .ThenInclude(a => a.Car)
                    .ThenInclude(c => c.Photos)
            .Include(u => u.Results)
                .ThenInclude(r => r.Tournament)
                    .ThenInclude(t => t.Track)
            .Include(u => u.OrganizedTournaments)
                .ThenInclude(o => o.Tournament)
                    .ThenInclude(t => t.Track)
            .Include(u => u.OrganizedTournaments)
                .ThenInclude(o => o.Tournament)
                    .ThenInclude(t => t.Applications)
            .Include(u => u.OrganizedTournaments)
                .ThenInclude(o => o.Tournament)
                    .ThenInclude(t => t.Photos)
            .Include(u => u.Avatar)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<(List<User> Items, int TotalCount)> GetPagedAsync(
            Expression<Func<User, bool>>? filter,
            int page,
            int pageSize)
    {
        var query = _context.Users
            .Include(u => u.Avatar)
            .AsQueryable();

        if (filter != null)
        {
            query = query.Where(filter);
        }

        return await query.ToPagedTupleAsync(page, pageSize);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .Include(u => u.Cars)
            .Include(u => u.Applications)
            .Include(u => u.Results)
            .Include(u => u.Avatar)
            .FirstOrDefaultAsync(u => u.Email == email);
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
