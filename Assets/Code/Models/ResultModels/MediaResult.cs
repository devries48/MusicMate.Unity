using System;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json;

[Serializable]
public class MediaResult
{
    [JsonProperty("mediaNumber")]
    public short Number { get; set; }
    public List<TrackResult> Tracks { get; set; }
}
