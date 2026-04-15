namespace MotorsportErp.Application.Common.Interfaces.Repositories;

public interface IBaseRepository<T>
{
    Task<T?> GetByIdAsync(Guid id);

    Task AddAsync(T entity);

    Task UpdateAsync(T entity);

    Task DeleteAsync(T entity);
}