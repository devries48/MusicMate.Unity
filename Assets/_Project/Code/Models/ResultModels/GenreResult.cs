using System;

public class GenreResult
{
    public Guid Id { get; set; }
    public string Text { get; set; }
    public bool IsMainGenre { get; set; } = false;
    public int SortOrder { get; set; } = 0;
    public int? ArtistCount { get; set; }
    public int? ReleaseCount { get; set; }
}