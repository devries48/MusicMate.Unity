using UnityEditor;

[CustomEditor(typeof(ToolbarButtonAnimator))]
public class ToolbarButtonAnimatorEditor : MusicMateEditorBase
{
    public override void OnInspectorGUI()
    {
        var _buttonType = serializedObject.FindProperty("_buttonType");
        var buttonType = (ToolbarButtonType)_buttonType.enumValueIndex;

        serializedObject.Update();

        DrawLogo();
        DrawTitle("ToolbarButton Animator");

        DrawSectionField(_buttonType, "Button Type");
        DrawSectionField("_interactable", "Is Interactable");

        if (buttonType != ToolbarButtonType.ToggleText)
            DrawSectionField("_icon", "Icon");

        DrawSectionField("_tooltip", "Tooltip");

        DrawSpace();

        switch (buttonType)
        {
            case ToolbarButtonType.ToggleText:
                DrawSectionHeader("Toggle Text Button");
                DrawSectionField("_text", "Text");
                DrawSectionField("_isToggleOn", "Toggle On");
                DrawSectionField("_isToggleGroup", "Member of Toggle-group");
                break;
            case ToolbarButtonType.Toggle:
                DrawSectionHeader("Toggle Button");
                DrawSectionField("_isToggleOn", "Toggle On");
                DrawSectionField("_isToggleGroup", "Member of Toggle-group");
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}