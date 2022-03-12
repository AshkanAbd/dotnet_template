using Infrastructure.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Pagination;

public static class PaginationTools
{
    private static void NormalizePagination(ref int? page, ref int? pageSize)
    {
        if (page <= 0) {
            page = 1;
        }

        if (pageSize == 0 || pageSize < -1) {
            pageSize = 10;
        }
    }

    private static void LoadPaginationFromRequest(ref int? page, ref int? pageSize, HttpRequest request)
    {
        var pageQueryParam = request.Query
            .Where(x => x.Key == "page")
            .Select(x => x.Value)
            .FirstOrDefault();
        if (!pageQueryParam.IsNullOrEmpty()) {
            if (int.TryParse(pageQueryParam, out var iPage)) {
                page = iPage;
            }
        }

        var pageSizeQueryParam = request.Query
            .Where(x => x.Key == "pageSize")
            .Select(x => x.Value)
            .FirstOrDefault();

        if (pageSizeQueryParam.IsNullOrEmpty()) return;

        if (int.TryParse(pageSizeQueryParam, out var iPageSize)) {
            pageSize = iPageSize;
        }
    }

    public static async Task<PaginationModel<T>> UsePaginationAsync<T>(this IQueryable<T> query, int? page = null,
        int? pageSize = null, CancellationToken cancellationToken = default)
    {
        NormalizePagination(ref page, ref pageSize);
        var iPage = page ?? 1;
        var iPageSize = pageSize ?? 1;
        var count = query.Count();

        if (iPageSize != -1) {
            query = query.Skip((iPage - 1) * iPageSize).Take(iPageSize);
        }
        else {
            iPageSize = count == 0 ? 1 : count;
        }

        return new PaginationModel<T> {
            List = await query.ToListAsync(cancellationToken),
            PageCount = (int) Math.Ceiling(count / (double) iPageSize),
            CurrentPage = iPage,
            CurrentPageSize = iPageSize,
            Total = count
        };
    }

    public static async Task<PaginationModel<T>> UsePaginationAsync<T>(this IQueryable<T> query,
        HttpRequest request,
        CancellationToken cancellationToken = default)
    {
        int? page = 1;
        int? pageSize = 10;
        if (request != null) {
            LoadPaginationFromRequest(ref page, ref pageSize, request);
        }

        return await UsePaginationAsync(query, page, pageSize, cancellationToken);
    }

    public static PaginationModel<T> UsePagination<T>(this IQueryable<T> query, int? page = null,
        int? pageSize = null)
    {
        NormalizePagination(ref page, ref pageSize);
        var iPage = page ?? 1;
        var iPageSize = pageSize ?? 1;
        var count = query.Count();

        if (iPageSize != -1) {
            query = query.Skip((iPage - 1) * iPageSize).Take(iPageSize);
        }
        else {
            iPageSize = count == 0 ? 1 : count;
        }

        return new PaginationModel<T> {
            List = query.ToList(),
            PageCount = (int) Math.Ceiling(count / (double) iPageSize),
            CurrentPage = iPage,
            CurrentPageSize = iPageSize,
            Total = count
        };
    }

    public static PaginationModel<T> UsePagination<T>(this IQueryable<T> query, HttpRequest request)
    {
        int? page = 1;
        int? pageSize = 10;

        if (request != null) {
            LoadPaginationFromRequest(ref page, ref pageSize, request);
        }

        return UsePagination(query, page, pageSize);
    }
}