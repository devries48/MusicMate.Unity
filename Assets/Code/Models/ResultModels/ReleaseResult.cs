using System;
using System.Collections.Generic;

public class ReleaseResult
{
    public Guid Id { get; set; } = Guid.Empty;
    public Statuses Status { get; set; }
    public WorkflowStatus? WorkflowStatus { get; set; }
    public int DatabaseId { get; set; }
    public DataResult Artist { get; set; }
    public string Country { get; set; }
    public string Title { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string Genre { get; set; }
    public decimal Duration { get; set; }
    public string DurationTime { get; set; }
    public string ThumbnailUrl { get; set; }
    public short? Rating { get; set; }
    public List<MediaResult> Media { get; set; }
}
