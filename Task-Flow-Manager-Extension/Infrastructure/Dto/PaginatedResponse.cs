namespace Task_Flow_Manager_Extension.Infrastructure.Dto;

public class PaginatedResponse<T>(int totalCount, int page, int pageSize, IEnumerable<T> items)
{
    public int Page { get; set; } = page;
    public int PageSize { get; set; } = pageSize;
    public int TotalCount { get; set; } = totalCount;
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public IEnumerable<T> Items { get; set; } = items;
}