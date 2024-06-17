using System.Collections.Generic;

public class PagedResult<T>
{
    public int? CurrentPage { get; set; }
    public bool? IsSuccess { get; set; }
    public string Message { get; set; }
    public int? OperationTime { get; set; }
    public List<T> Rows { get; set; }
    public int? TotalCount { get; set; }
    public int? TotalPages { get; set; }
}