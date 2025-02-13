using System;
using System.Collections.Generic;

public class ArtistModel : ArtistResult
{
    public DateTime CreatedDate { get; set; }
    public object LastUpdated { get; set; }
    public string AmgId { get; set; }
    public string ArtistType { get; set; }
    public string BandStatus { get; set; }
    public string BioContext { get; set; }
    public DateTime? BirthDate { get; set; }
    public string DiscogsId { get; set; }
    public IEnumerable<GenreResult> Genres { get; set; }
    public IEnumerable<string> IsniList { get; set; }
    public string ITunesId { get; set; }
    public string MusicBrainzId { get; set; }
    public string Profile { get; set; }
    public double? Rank { get; set; }
    public int? RankPosition { get; set; }
    public string RealName { get; set; }
    public List<ReleaseResult> Releases { get; set; }
    public string SortNameValue { get; set; }
    public string SpotifyId { get; set; }
}
