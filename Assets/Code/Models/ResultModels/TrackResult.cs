using System;
using Newtonsoft.Json;

[Serializable]
public class TrackResult
{
    public DataResult TrackArtist { get; set; }

    [JsonProperty("trackNumber")]
    public int Number { get; set; }

    public string Title { get; set; }

    public int Duration { get; set; }

    public string TrackPlayUrl { get; set; }

    public string DurationString
    {
        get
        {
            if(Duration < 0)
                return "0:00";

            int totalSeconds = Duration / 1000;

            int currentSeconds = totalSeconds % 60;
            int minutes = totalSeconds / 60;

            string secondsStr = "";
            if(currentSeconds < 10)
                secondsStr = "0";
            
            secondsStr += currentSeconds.ToString();

            return $"{minutes}:{secondsStr}";
        }
    }
}
