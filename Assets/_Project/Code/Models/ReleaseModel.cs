using System;
using System.Collections.Generic;

public class ReleaseModel : ReleaseResult
{
    public string SubTitle { get; set; }
    public string SortTitle { get; set; }
    public ReleaseType? ReleaseType { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime LastUpdated { get; set; }
    public double? Rank { get; set; }
    public string AmgId { get; set; }
    public string DiscogsId { get; set; }
    public string ItunesId { get; set; }
    public string LastFmId { get; set; }
    public string RateYourMusicId { get; set; }
    public string MusicBrainzId { get; set; }
    public string SpotifyId { get; set; }

    public string Tags { get; set; } = string.Empty;
    public string URLs { get; set; } = string.Empty;
    public string Info { get; set; }
    
    public IEnumerable<GenreResult> Genres { get; set; }
    public IEnumerable<ReleaseLabelResult> Labels { get; set; }
    public IEnumerable<ImageModel> Images { get; set; }
    /// <summary>
    /// The Position of this Release as ranked against other Releases (highest ranking Release is #1)
    /// </summary>
    public int? RankPosition { get; set; }
}
