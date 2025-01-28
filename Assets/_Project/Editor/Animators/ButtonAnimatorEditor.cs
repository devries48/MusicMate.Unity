using UnityEditor;

[CustomEditor(typeof(ButtonAnimator))]
public class ButtonAnimatorEditor : MusicMateEditorBase
{
    public override void OnInspectorGUI()
    {
        var _buttonType = serializedObject.FindProperty("_buttonType");
        
        serializedObject.Update();

        DrawLogo();
        DrawTitle("Button Animator");
   
        DrawSectionField(_buttonType, "Button Type");
        DrawSectionField("_interactable", "Is Interactable");

        var buttonType = (ButtonType)_buttonType.enumValueIndex;
        if(buttonType != ButtonType.ExpandCollapse)
            DrawSectionField("_isPrimary", "Is Primary");

        DrawSpace();

        switch (buttonType)
        {
            case ButtonType.Text:
                DrawSectionHeader("Text Button");
                DrawSectionField("_text", "Text");
                break;
            case ButtonType.DefaultImage:
            case ButtonType.LargeImage:
                DrawSectionHeader("Image Button");
                DrawSectionField("_icon", "Icon");
                break;
            case ButtonType.StateImage:
                DrawSectionHeader("State Image Button");
                DrawSectionField("_isStateOn", "State ON");
                DrawSectionField("_icon", "State Icon OFF");
                DrawSectionField("_stateIcon", "State Icon ON");
                break;
            case ButtonType.ExpandCollapse:
                DrawSectionHeader("Expand/Collapse Button");
                DrawSectionField("_isToggle", "Is Toggle");
                DrawSectionField("_isExpanded", "Is Expanded");
                DrawSectionField("_headerText", "Header Text");
                break;
            default:
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
