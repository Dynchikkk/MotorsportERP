using Microsoft.EntityFrameworkCore;

namespace MotorsportErp.Infrastructure.Extensions;

public static class QueryableExtensions
{
    private const int MIN_PAGE = 0;
    private const int MIN_PAGE_SIZE = 1;
    private const int MAX_PAGE_SIZE = 100;

    public static async Task<(List<T> Items, int TotalCount)> ToPagedTupleAsync<T>(
        this IQueryable<T> query,
        int page,
        int pageSize)
    {
        page = Math.Max(page, MIN_PAGE);
        pageSize = Math.Min(Math.Max(pageSize, MIN_PAGE_SIZE), MAX_PAGE_SIZE);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}