using UnityEditor;

[CustomEditor(typeof(ToolbarButtonAnimator))]
public class ToolbarButtonAnimatorEditor : MusicMateEditorBase
{
    public override void OnInspectorGUI()
    {
        var _buttonType = serializedObject.FindProperty("_buttonType");

        serializedObject.Update();

        DrawLogo();
        DrawTitle("ToolbarButton Animator");
      
        DrawSectionField(_buttonType, "Button Type");
        DrawSectionField("_interactable", "Is Interactable");
        DrawSectionField("_icon", "Icon");
        DrawSectionField("_tooltip", "Tooltip");

        var buttonType = (ToolbarButtonType)_buttonType.enumValueIndex;

        DrawSpace();

        switch (buttonType)
        {
            case ToolbarButtonType.ToggleText:
                DrawSectionHeader("Toggle Text Button");
                DrawSectionField("_text", "Text");
                DrawSectionField("_isToggleOn", "Toggle On");
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