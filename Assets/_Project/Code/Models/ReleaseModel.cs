using System;
using System.Collections.Generic;

public class ReleaseModel : ReleaseResult
{
    public DateTime CreatedDate { get; set; }
    public DateTime LastUpdated { get; set; }
    public string AlternateNames { get; set; } 
    public string Tags { get; set; } = string.Empty;
    public string URLs { get; set; } = string.Empty;
    public IEnumerable<GenreResult> Genres { get; set; }
    public IEnumerable<ReleaseLabelResult> Labels { get; set; }
    public IEnumerable<DataResult> Images { get; set; }
    public string AmgId { get; set; }
    public string DiscogsId { get; set; }
    public string ItunesId { get; set; }
    public string LastFmId { get; set; }
    public string LastFmSummary { get; set; }
    public string MusicBrainzId { get; set; }
    public string SpotifyId { get; set; }
    public string Profile { get; set; }
    public double? Rank { get; set; }
    /// <summary>
    /// The Position of this Release as ranked against other Releases (highest ranking Release is #1)
    /// </summary>
    public int? RankPosition { get; set; }
    public string ReleaseType { get; set; }
    public string SortTitle { get; set; }
}
