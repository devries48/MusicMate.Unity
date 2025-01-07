using UnityEditor;

[CustomEditor(typeof(ButtonAnimations))]
public class ButtonAnimationsEditor : MusicMateEditorBase
{
    protected override void OnEnable()
    {
        base.OnEnable();

        InitializeFoldoutState("General Settings");
        InitializeFoldoutState("Text Button");
        InitializeFoldoutState("Default Image Button");
        InitializeFoldoutState("Large Image Button");
        InitializeFoldoutState("Toolbar Button");
        InitializeFoldoutState("Expand/Collapse Button");
        InitializeFoldoutState("Toolbar Spinner");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawLogo();
        DrawTitle("Button Animation Configuration");

        var _animationTime = serializedObject.FindProperty("_animationTime");
        var _animationEase = serializedObject.FindProperty("_animationEase");

        var _textHoverScale = serializedObject.FindProperty("_textHoverScale");
        var _textClickScale = serializedObject.FindProperty("_textClickScale");

        var _imageScale = serializedObject.FindProperty("_imageScale");
        var _imageHoverScale = serializedObject.FindProperty("_imageHoverScale");
        var _imageClickScale = serializedObject.FindProperty("_imageClickScale");

        var _imageLargeScale = serializedObject.FindProperty("_imageLargeScale");
        var _imageLargeHoverScale = serializedObject.FindProperty("_imageLargeHoverScale");
        var _imageLargeClickScale = serializedObject.FindProperty("_imageLargeClickScale");

        var _toolbarHoverScale = serializedObject.FindProperty("_toolbarHoverScale");
        var _toolbarClickScale = serializedObject.FindProperty("_toolbarClickScale");
        var _toolbarToggleScale = serializedObject.FindProperty("_toolbarToggleScale");
        var _toolbarTooltipPopupTime = serializedObject.FindProperty("_toolbarTooltipPopupTime");

        var _iconAnimationTime = serializedObject.FindProperty("_iconAnimationTime");
        var _toolbarSpinnerScale = serializedObject.FindProperty("_toolbarSpinnerScale");
        var _toolbarSpinTime = serializedObject.FindProperty("_toolbarSpinTime");

        if (DrawFoldout("General Settings"))
        {
            DrawSectionField(_animationTime, "Default Animation Duration");
            DrawSectionField(_animationEase, "Animation Ease");
        }

        if (DrawFoldout("Text Button"))
        {
            DrawSectionField(_textHoverScale, "Hover Scale");
            DrawSectionField(_textClickScale, "Click Scale");
        }

        if (DrawFoldout("Default Image Button"))
        {
            DrawSectionField(_imageScale, "Scale");
            DrawSectionField(_imageHoverScale, "Hover Scale");
            DrawSectionField(_imageClickScale, "Click Scale");
        }

        if (DrawFoldout("Large Image Button"))
        {
            DrawSectionField(_imageLargeScale, "Scale");
            DrawSectionField(_imageLargeHoverScale, "Hover Scale");
            DrawSectionField(_imageLargeClickScale, "Click Scale");
        }

        if (DrawFoldout("Toolbar Button"))
        {
            DrawSectionField(_toolbarHoverScale, "Hover Scale");
            DrawSectionField(_toolbarClickScale, "Click Scale");
            DrawSectionField(_toolbarToggleScale, "Toggle Scale");
            DrawSectionField(_toolbarTooltipPopupTime, "Tooltip Popup Time");
        }

        if (DrawFoldout("Expand/Collapse Button"))
        {
            DrawSectionField(_iconAnimationTime, "Icon Animation Time");
        }

        if (DrawFoldout("Toolbar Spinner"))
        {
            DrawSectionField(_toolbarSpinnerScale, "Spinner Scale");
            DrawSectionField(_toolbarSpinTime, "Spin Time");
        }
        
        serializedObject.ApplyModifiedProperties();
    }
}
