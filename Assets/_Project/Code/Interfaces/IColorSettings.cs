using UnityEngine;

public interface IColorSettings
{
    Color32 AccentColor { get; }
    Color32 DefaultColor { get; }
    Color32 TextColor { get; }
    Color32 PanelColor { get; }
    Color32 BackgroundColor { get; }

    Color32 AccentTextColor { get; }
    Color32 DisabledTextColor { get; }
    Color32 IconColor { get; }
    Color32 DisabledIconColor { get; }
}
