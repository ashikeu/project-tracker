namespace ProjectTracker.Shared.Results;

public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; }
    public int Page { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);

    public PagedResult(IReadOnlyList<T> items, int page, int pageSize, int totalCount)
    {
        Items = items;
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
    }
}

public class PagingRequest
{
    private const int MaxPageSize = 200;
    private int _pageSize = 50;

    public int Page { get; set; } = 1;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value <= 0 ? 50 : Math.Min(value, MaxPageSize);
    }

    public int Skip => (Math.Max(Page, 1) - 1) * PageSize;
}
