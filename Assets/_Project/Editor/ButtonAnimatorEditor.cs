using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ButtonAnimator))]
public class ButtonAnimatorEditor : MusicMateEditorBase
{
    // Serialized properties
    private SerializedProperty _buttonType;
    private SerializedProperty _interactable;
    private SerializedProperty _isPrimary;
    private SerializedProperty _text;
    private SerializedProperty _icon;
    private SerializedProperty _isToggle;
    private SerializedProperty _isExpanded;
    private SerializedProperty _headerText;

    protected override void OnEnable()
    {
        base.OnEnable();

        // Cache serialized properties
        _buttonType = serializedObject.FindProperty("_buttonType");
        _interactable = serializedObject.FindProperty("_interactable");
        _isPrimary = serializedObject.FindProperty("_isPrimary");
        _text = serializedObject.FindProperty("_text");
        _icon = serializedObject.FindProperty("_icon");
        _isToggle = serializedObject.FindProperty("_isToggle");
        _isExpanded = serializedObject.FindProperty("_isExpanded");
        _headerText = serializedObject.FindProperty("_headerText");
    }

    public override void OnInspectorGUI()
    {
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
