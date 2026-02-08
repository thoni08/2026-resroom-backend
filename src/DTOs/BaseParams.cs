namespace ResRoomApi.DTOs;

public class BaseParams
{
    // Pagination Settings
    private const int MaxPageSize = 50;
    public int PageNumber { get; set; } = 1;
    
    private int _pageSize = 10;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }

    // Filters
    public string? SearchTerm { get; set; }

    // Sorting
    public string? SortBy { get; set; }
    public string SortDirection { get; set; } = "asc"; // "asc" or "desc"
}