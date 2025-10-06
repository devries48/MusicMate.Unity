using System;
using System.Collections.Generic;
using System.Linq;

public class ReleaseResult
{
    public Guid Id { get; set; } = Guid.Empty;
    public ReleaseStatus Status { get; set; }
    public int DatabaseId { get; set; }
    public DataResult Artist { get; set; }
    public string Country { get; set; }
    public string Title { get; set; }
    public int ReleaseYear { get; set; }
    public string MainGenre { get; set; }
    public decimal? Duration { get; set; }
    public string DurationTime { get; set; }
    public string ThumbnailUrl { get; set; }
    public short? Rating { get; set; }
    public List<MediaResult> Media { get; set; }

    public List<TrackResult> GetAllTracks()
    {
        var tracks= Media?.Where(media => media.Tracks != null)
                     .SelectMany(media => media.Tracks)
                     .ToList() ?? new List<TrackResult>();

        foreach (var track in tracks)
            track.Release = this;

        return tracks;
    }
}
