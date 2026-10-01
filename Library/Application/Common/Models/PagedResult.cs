namespace Library.Application.Common.Models;

public sealed class PagedResult<T>
{
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public IReadOnlyCollection<T> Items { get; init; } = [];
}
