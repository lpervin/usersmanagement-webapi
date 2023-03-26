namespace users_webapi.Models;

public class PagedResponse<T>
{
    public long TotalRecordsCount { get; set; }
    public int PageRecordsCount { get; set; }
    public int PageCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public SortInfo OrderBy { get; set; }
    public IReadOnlyList<T> PageData { get; set; }
}
