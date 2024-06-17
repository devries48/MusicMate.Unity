using TMPro;
using UnityEngine;

public class PlaylistItemController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _nrText;
    [SerializeField] Marquee _titleMarque;
    [SerializeField] TextMeshProUGUI _durationText;

    public void Initialize(TrackResult track,int pos)
    {
        _nrText.text = pos.ToString();
        _titleMarque.SetText(track.Title);
        _durationText.text = track.DurationString;
    }

    public void SetColor(Color32 color)
    {
        _nrText.color = color;
        _titleMarque.SetColor(color);
        _durationText.color = color;
    }    
}
