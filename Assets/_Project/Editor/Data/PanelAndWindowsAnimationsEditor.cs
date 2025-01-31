using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PanelAnimations))]
public class PanelAndWindowsAnimationsEditor : MusicMateEditorBase
{
    protected override void OnEnable()
    {
        base.OnEnable();

        InitializeFoldoutState("Default Window Visibility");
        InitializeFoldoutState("Default Panel Fading");
        InitializeFoldoutState("Login Window");
        InitializeFoldoutState("Error Window");
        InitializeFoldoutState("Result Grid");
        InitializeFoldoutState("Details Panel");
        InitializeFoldoutState("Providers Panel");
        InitializeFoldoutState("AudioPlayer Collapsed");
        InitializeFoldoutState("AudioPlayer Expanded");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawLogo();
        DrawTitle("Panel and Window Animation Configuration");

        if (DrawFoldout("Default Window Visibility"))
        {
            DrawSectionField("_showAndHideDuration", "Show /Hide Duration");
            DrawSectionField("_hideDelay", "Hide Delay");
            DrawSectionField("_showEase", "Show Ease");
            DrawSectionField("_hideEase", "Hide Ease");
        }

        if (DrawFoldout("Default Panel Fading"))
        {
            DrawSectionField("_fadeDuration", "Fade Duration");
            DrawSectionField("_fadeEase", "Fade Ease");
        }

        if (DrawFoldout("Login Window"))
        {
            DrawSectionField("_loginHidePivot", "Hide Pivot Y");
            DrawSectionField("_loginShowPivot", "Show Pivot Y");
        }

        if (DrawFoldout("Error Window"))
        {
            DrawSectionField("_errorHidePivot", "Hide Pivot Y");
            DrawSectionField("_errorShowPivot", "Show Pivot Y");
        }

        if (DrawFoldout("Result Grid"))
        {
            DrawSectionField("_resultShowTime", "Show Time");
            DrawSectionField("_resultShowEase", "Show Ease");
            DrawSectionField("_resultHideTime", "Hide Time");
            DrawSectionField("_resultHideScaleTo", "Hide Scale To");
            DrawSectionField("_resultHideFadeTo", "Hide Fade To");
            DrawSectionField("_resultHideEase", "Hide Ease");
        }

        if (DrawFoldout("Details Panel"))
        {
            DrawSectionField("_detailsShowTime", "Show Time");
            DrawSectionField("_detailsShowEase", "Show Ease");
            DrawSectionField("_detailsHideTime", "Hide Time");
            DrawSectionField("_detailsHideEase", "Hide Ease");
        }

        if (DrawFoldout("Providers Panel"))
        {
            DrawSectionField("_providersShowTime", "Show Time");
            DrawSectionField("_providersShowEase", "Show Ease");
            DrawSectionField("_providersShowPivot", "Show Pivot X");
            DrawSectionField("_providersHideTime", "Hide Time");
            DrawSectionField("_providersHideEase", "Hide Ease");
            DrawSectionField("_providersHidePivot", "Hide Pivot X");
        }

        if (DrawFoldout("AudioPlayer Collapsed"))
        {
            DrawSectionField("_playerCollapseTime", "Collapse Time");
            DrawSectionField("_playerSmallShowDelay", "Small Show Delay");
            DrawSectionField("_playerLargeShrinkEase", "Large Shrink Ease");
            DrawSectionField("_playerLargeHideEase", "Large Hide Ease");
            DrawSectionField("_playerSmallShowEase", "Small Show Ease");
        }

        if (DrawFoldout("AudioPlayer Expanded"))
        {
            DrawSectionField("_playerExpandTime", "Expand Time");
            DrawSectionField("_playerLargeGrowDelay", "Large Grow Delay");
            DrawSectionField("_playerSmallHideEase", "Small Hide Ease");
            DrawSectionField("_playerLargeShowEase", "Large Show Ease");
            DrawSectionField("_playerLargeGrowEase", "Large Grow Ease");
        }

        serializedObject.ApplyModifiedProperties();
    }
}
