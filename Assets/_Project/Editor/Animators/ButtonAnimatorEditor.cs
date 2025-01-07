using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ButtonAnimator))]
public class ButtonAnimatorEditor : MusicMateEditorBase
{
    public override void OnInspectorGUI()
    {
        // Cache serialized properties
        var _buttonType = serializedObject.FindProperty("_buttonType");
        var _interactable = serializedObject.FindProperty("_interactable");
        var _isPrimary = serializedObject.FindProperty("_isPrimary");
        var _text = serializedObject.FindProperty("_text");
        var _icon = serializedObject.FindProperty("_icon");
        var _isToggle = serializedObject.FindProperty("_isToggle");
        var _isExpanded = serializedObject.FindProperty("_isExpanded");
        var _headerText = serializedObject.FindProperty("_headerText");
        
        serializedObject.Update();

        DrawLogo();
        DrawSectionHeader("Button Animator");
        DrawSectionField(_buttonType, "Button Type");
        DrawSectionField(_interactable, "Is Interactable");

        var buttonType = (ButtonAnimationType)_buttonType.enumValueIndex;
        if(buttonType != ButtonAnimationType.ExpandCollapseButton)
            DrawSectionField(_isPrimary, "Is Primary");

        DrawSpace();

        switch (buttonType)
        {
            case ButtonAnimationType.TextButton:
                DrawSectionHeader("Text Button");
                DrawSectionField(_text, "Text");
                break;
            case ButtonAnimationType.DefaultImageButton:
            case ButtonAnimationType.LargeImageButton:
                DrawSectionHeader("Image Button");
                DrawSectionField(_icon, "Icon");
                break;
            case ButtonAnimationType.ExpandCollapseButton:
                DrawSectionHeader("Expand/Collapse Button");
                DrawSectionField(_isToggle, "Is Toggle");
                DrawSectionField(_isExpanded, "Is Expanded");
                DrawSectionField(_headerText, "Header Text");

                break;
            case ButtonAnimationType.ToolbarButton:
                GUILayout.Label("Toolbar buttons have their own custom behavior.", EditorStyles.boldLabel);
                break;
            default:
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
