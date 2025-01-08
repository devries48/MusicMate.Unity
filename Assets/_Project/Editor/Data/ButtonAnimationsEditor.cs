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
        InitializeFoldoutState("Expand/Collapse Button");
        InitializeFoldoutState("Toolbar Button");
        InitializeFoldoutState("Toolbar Spinner");
        InitializeFoldoutState("Toolbar Part Rotation");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawLogo();
        DrawTitle("Button Animation Configuration");

        if (DrawFoldout("General Settings"))
        {
            DrawSectionField("_animationTime", "Default Animation Duration");
            DrawSectionField("_animationEase", "Animation Ease");
        }

        if (DrawFoldout("Text Button"))
        {
            DrawSectionField("_textHoverScale", "Hover Scale");
            DrawSectionField("_textClickScale", "Click Scale");
        }

        if (DrawFoldout("Default Image Button"))
        {
            DrawSectionField("_imageScale", "Scale");
            DrawSectionField("_imageHoverScale", "Hover Scale");
            DrawSectionField("_imageClickScale", "Click Scale");
        }

        if (DrawFoldout("Large Image Button"))
        {
            DrawSectionField("_imageLargeScale", "Scale");
            DrawSectionField("_imageLargeHoverScale", "Hover Scale");
            DrawSectionField("_imageLargeClickScale", "Click Scale");
        }

        if (DrawFoldout("Expand/Collapse Button"))
        {
            DrawSectionField("_iconAnimationTime", "Icon Animation Time");
        }

        if (DrawFoldout("Toolbar Button"))
        {
            DrawSectionField("_toolbarHoverScale", "Hover Scale");
            DrawSectionField("_toolbarClickScale", "Click Scale");
            DrawSectionField("_toolbarToggleScale", "Toggle Scale");
            DrawSectionField("_toolbarTooltipPopupTime", "Tooltip Popup Time");
        }

        if (DrawFoldout("Toolbar Spinner"))
        {
            DrawSectionField("_toolbarSpinnerScale", "Spinner Scale");
            DrawSectionField("_toolbarSpinTime", "Spin Time");
        }

        if (DrawFoldout("Toolbar Part Rotation"))
        {
            DrawSectionField("_toolbarPartRotateTime", "Part Rotation Time");
            DrawSectionField("_toolbarPartDelayTime", "Part Delay Time");
        }

        serializedObject.ApplyModifiedProperties();
    }
}
