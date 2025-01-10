using UnityEngine;

[CreateAssetMenu(menuName = "MusicMate/Settings/Color Settings", fileName = "Color Settings")]
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