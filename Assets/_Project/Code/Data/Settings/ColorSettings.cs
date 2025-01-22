using UnityEngine;

[CreateAssetMenu(menuName = "MusicMate/Settings/Color Settings", fileName = "Color Settings")]
public class ColorSettings : ScriptableObject, IColorSettings
{
    [SerializeField] private Color32 _accentColor;
    [SerializeField] private Color32 _defaultColor;
    [SerializeField] private Color32 _textColor;
    [SerializeField] private Color32 _backgroundColor;
    [SerializeField] private Color32 _panelColor;

    public Color32 AccentColor => _accentColor;
    public Color32 DefaultColor => _defaultColor;
    public Color32 TextColor => _textColor;
    public Color32 BackgroundColor => _backgroundColor;
    public Color32 PanelColor => _panelColor;

    public Color32 AccentTextColor => _backgroundColor;
    public Color32 IconColor => _textColor;
    public Color32 DisabledIconColor => _defaultColor;
}