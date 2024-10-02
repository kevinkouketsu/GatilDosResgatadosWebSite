using GatilDosResgatadosApi.Areas.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GatilDosResgatadosApi.Infrastructure;

public static class LinqExtensions
{
    public static async Task<PaginatedList<T>> ToPaginatedListAsync<T>(this IQueryable<T> source, IPageable pageable, CancellationToken cancellationToken = default) 
    {
        int currentPage = pageable.PageNumber;
        int pageSize = pageable.PageSize;

        int totalItems = await source.CountAsync(cancellationToken);
        int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        var items = await source
            .Skip((currentPage - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new()
        {
            CurrentPage = currentPage,
            TotalPages = totalPages,
            TotalItems = totalItems,
            Result = items
        };
    }

    public static IQueryable<T> WhereIf<T>(this IQueryable<T> source, bool condition, Expression<Func<T, bool>> predicate)
    {
        return condition ? source.Where(predicate) : source;
    }
}
