using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PanelAndWindowsAnimations))]
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
        InitializeFoldoutState("AudioPlayer Collapsed");
        InitializeFoldoutState("AudioPlayer Expanded");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawLogo();
        DrawTitle("Panel and Window Animation Configuration");

        // Find properties
        var _showAndHideDuration = serializedObject.FindProperty("_showAndHideDuration");
        var _hideDelay = serializedObject.FindProperty("_hideDelay");
        var _showEase = serializedObject.FindProperty("_showEase");
        var _hideEase = serializedObject.FindProperty("_hideEase");

        var _fadeDuration = serializedObject.FindProperty("_fadeDuration");
        var _fadeEase = serializedObject.FindProperty("_fadeEase");

        var _loginHidePivot = serializedObject.FindProperty("_loginHidePivot");
        var _loginShowPivot = serializedObject.FindProperty("_loginShowPivot");

        var _errorHidePivot = serializedObject.FindProperty("_errorHidePivot");
        var _errorShowPivot = serializedObject.FindProperty("_errorShowPivot");

        var _resultShowTime = serializedObject.FindProperty("_resultShowTime");
        var _resultShowEase = serializedObject.FindProperty("_resultShowEase");
        var _resultHideTime = serializedObject.FindProperty("_resultHideTime");
        var _resultHideScaleTo = serializedObject.FindProperty("_resultHideScaleTo");
        var _resultHideFadeTo = serializedObject.FindProperty("_resultHideFadeTo");
        var _resultHideEase = serializedObject.FindProperty("_resultHideEase");

        var _detailsShowTime = serializedObject.FindProperty("_detailsShowTime");
        var _detailsShowEase = serializedObject.FindProperty("_detailsShowEase");
        var _detailsHideTime = serializedObject.FindProperty("_detailsHideTime");
        var _detailsHideEase = serializedObject.FindProperty("_detailsHideEase");

        var _playerCollapseTime = serializedObject.FindProperty("_playerCollapseTime");
        var _playerSmallShowDelay = serializedObject.FindProperty("_playerSmallShowDelay");
        var _playerLargeShrinkEase = serializedObject.FindProperty("_playerLargeShrinkEase");
        var _playerLargeHideEase = serializedObject.FindProperty("_playerLargeHideEase");
        var _playerSmallShowEase = serializedObject.FindProperty("_playerSmallShowEase");

        var _playerExpandTime = serializedObject.FindProperty("_playerExpandTime");
        var _playerLargeGrowDelay = serializedObject.FindProperty("_playerLargeGrowDelay");
        var _playerSmallHideEase = serializedObject.FindProperty("_playerSmallHideEase");
        var _playerLargeShowEase = serializedObject.FindProperty("_playerLargeShowEase");
        var _playerLargeGrowEase = serializedObject.FindProperty("_playerLargeGrowEase");

        // Draw foldouts and sections
        if (DrawFoldout("Default Window Visibility"))
        {
            DrawSectionField(_showAndHideDuration, "Show/Hide Duration");
            DrawSectionField(_hideDelay, "Hide Delay");
            DrawSectionField(_showEase, "Show Ease");
            DrawSectionField(_hideEase, "Hide Ease");
        }

        if (DrawFoldout("Default Panel Fading"))
        {
            DrawSectionField(_fadeDuration, "Fade Duration");
            DrawSectionField(_fadeEase, "Fade Ease");
        }

        if (DrawFoldout("Login Window"))
        {
            DrawSectionField(_loginHidePivot, "Hide Pivot Y");
            DrawSectionField(_loginShowPivot, "Show Pivot Y");
        }

        if (DrawFoldout("Error Window"))
        {
            DrawSectionField(_errorHidePivot, "Hide Pivot Y");
            DrawSectionField(_errorShowPivot, "Show Pivot Y");
        }

        if (DrawFoldout("Result Grid"))
        {
            DrawSectionField(_resultShowTime, "Show Time");
            DrawSectionField(_resultShowEase, "Show Ease");
            DrawSectionField(_resultHideTime, "Hide Time");
            DrawSectionField(_resultHideScaleTo, "Hide Scale To");
            DrawSectionField(_resultHideFadeTo, "Hide Fade To");
            DrawSectionField(_resultHideEase, "Hide Ease");
        }

        if (DrawFoldout("Details Panel"))
        {
            DrawSectionField(_detailsShowTime, "Show Time");
            DrawSectionField(_detailsShowEase, "Show Ease");
            DrawSectionField(_detailsHideTime, "Hide Time");
            DrawSectionField(_detailsHideEase, "Hide Ease");
        }

        if (DrawFoldout("AudioPlayer Collapsed"))
        {
            DrawSectionField(_playerCollapseTime, "Collapse Time");
            DrawSectionField(_playerSmallShowDelay, "Small Show Delay");
            DrawSectionField(_playerLargeShrinkEase, "Large Shrink Ease");
            DrawSectionField(_playerLargeHideEase, "Large Hide Ease");
            DrawSectionField(_playerSmallShowEase, "Small Show Ease");
        }

        if (DrawFoldout("AudioPlayer Expanded"))
        {
            DrawSectionField(_playerExpandTime, "Expand Time");
            DrawSectionField(_playerLargeGrowDelay, "Large Grow Delay");
            DrawSectionField(_playerSmallHideEase, "Small Hide Ease");
            DrawSectionField(_playerLargeShowEase, "Large Show Ease");
            DrawSectionField(_playerLargeGrowEase, "Large Grow Ease");
        }

        serializedObject.ApplyModifiedProperties();
    }
}
