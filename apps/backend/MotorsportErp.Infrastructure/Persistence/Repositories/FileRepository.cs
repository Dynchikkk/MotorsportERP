using Microsoft.EntityFrameworkCore;
using MotorsportErp.Application.Common.Interfaces.Repositories;
using MotorsportErp.Domain.Files;

namespace MotorsportErp.Infrastructure.Persistence.Repositories;
public class FileRepository : IFileRepository
{
    private readonly AppDbContext _context;

    public FileRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<MediaFile?> GetByIdAsync(Guid id)
    {
        return await _context.MediaFiles
            .Include(f => f.UploadedBy)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task AddAsync(MediaFile entity)
    {
        _ = await _context.MediaFiles.AddAsync(entity);
        _ = await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(MediaFile entity)
    {
        if (_context.Entry(entity).State == EntityState.Detached)
        {
            _ = _context.MediaFiles.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        _ = await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(MediaFile entity)
    {
        _ = _context.MediaFiles.Remove(entity);
        _ = await _context.SaveChangesAsync();
    }
}
