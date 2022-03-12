namespace Application.Common.Pagination;

public class PaginationModel<T>
{
    public IEnumerable<T> List { get; set; }
    public int PageCount { get; set; }
    public int CurrentPage { get; set; }
    public int CurrentPageSize { get; set; }
    public int Total { get; set; }
}