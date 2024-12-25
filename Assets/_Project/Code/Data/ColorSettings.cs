using UnityEngine;

[CreateAssetMenu(menuName = "MusicMate/Colors", fileName = "Colors")]
public class ColorSettings : ScriptableObject
{
    public Color32 AccentColor;
    public Color32 DefaultColor;
    public Color32 TextColor;
    public Color32 BackgroundColor;

    public Color32 AccentTextColor => BackgroundColor;
    public Color32 IconColor => TextColor;
    public Color32 DisabledIconColor => DefaultColor;
}