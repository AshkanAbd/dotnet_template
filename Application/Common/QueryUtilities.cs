using System.Linq.Expressions;
using SoftDeletes.ModelTools;

namespace Application.Common;

public static class QueryUtilities
{
    public static IOrderedQueryable<TSource> OrderByNewest<TSource>(this IQueryable<TSource> source)
        where TSource : ITimestamps => source.OrderByDescending(x => x.CreatedAt);

    public static IOrderedQueryable<TSource> OrderByOldest<TSource>(this IQueryable<TSource> source)
        where TSource : ITimestamps => source.OrderBy(x => x.CreatedAt);

    public static async Task<List<TResult>> ChartBuilder<TSource, TResult>(this IQueryable<TSource> queryable,
        DateTime startDate, DateTime endDate, TimeSpan step,
        Func<IQueryable<TSource>, DateTime, Task<TResult>> selectFunction)
        where TSource : ModelExtension
    {
        var chartList = new List<TResult>();
        var date1 = startDate;
        var date2 = startDate.ToPersianDateTime().Add(step).FirstDayOfMonth.ToDateTime();

        while (true) {
            if (date1 > endDate) {
                break;
            }

            chartList.Add(
                await selectFunction(
                    queryable.Where(x => x.CreatedAt > date1 && x.CreatedAt < date2),
                    date1
                )
            );

            date1 = date1.ToPersianDateTime().Add(step).FirstDayOfMonth.ToDateTime();
            date2 = date2.ToPersianDateTime().Add(step).FirstDayOfMonth.ToDateTime();
        }

        chartList.Reverse();
        return chartList;
    }

    public static IQueryable<TSource> WhereIfNotNull<TSource, TE>(this IQueryable<TSource> query, TE obj,
        Expression<Func<TSource, bool>> filter)
    {
        return obj switch {
            null => query,
            not null => query.Where(filter),
        };
    }
}