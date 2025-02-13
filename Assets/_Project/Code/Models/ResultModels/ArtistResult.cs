using System;

public class ArtistResult
{
    public string FormedCountry { get; set; }
    public int? FormedYear { get; set; }
    public Guid Id { get; set; } = Guid.Empty;
    public string Name { get; set; }
    public string SortName { get; set; }
    public Statuses? Status { get; set; }
    public string ThumbnailUrl { get; set; }
}

