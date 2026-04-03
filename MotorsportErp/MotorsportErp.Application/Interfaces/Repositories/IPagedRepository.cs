using System.Linq.Expressions;

namespace MotorsportErp.Application.Interfaces.Repositories;

public interface IPagedRepository<T>
{
    Task<(List<T> Items, int TotalCount)> GetPagedAsync(Expression<Func<T, bool>>? filter, int page, int pageSize);
}