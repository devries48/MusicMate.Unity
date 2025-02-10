using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ShowReleaseController))]
public class ShowReleaseControllerEditor : MusicMateEditorBase
{
    protected override void OnEnable()
    {
        base.OnEnable();

        InitializeFoldoutState("State Actions");
        InitializeFoldoutState("Elements");
        InitializeFoldoutState("Panels");
        InitializeFoldoutState("Buttons");
    }

    public override void OnInspectorGUI()
    {
        var controller = (ShowReleaseController)target;

        serializedObject.Update();

        DrawLogo();
        DrawTitle("Show Release Controller");

        DrawSectionHeader("Parent & State Objects");
        DrawSectionField("_parent", "Parent");
        DrawSectionField("m_normal", "Normal State");
        DrawSectionField("m_maximized", "Maximized State");

        DrawSpace();
        DrawSectionHeader("Mappings");
        DrawSpace();

        if (DrawFoldout("Elements"))
        {
            DrawSectionField("_image", "Release Image");
            DrawSectionField("_artist", "Artist");
            DrawSectionField("_title", "Title");
            DrawSectionField("_yearCountry", "Tear & Country");
            DrawSectionField("_mainGenre", "Main Genre");
            DrawSectionField("_subGenres", "Sub Genres");
            DrawSectionField("m_artist_title", "Artist & Title");
            DrawSectionField("m_total_length", "Total Length");
            DrawSectionField("m_tracks", "Tracks");
        }
        if (DrawFoldout("Panels"))
        {
            DrawSectionField("m_imagePanel", "Image Panel");
            DrawSectionField("m_mainInfoPanel", "Main Info Panel");
        }
        if (DrawFoldout("Buttons"))
        {
            DrawSectionField("_stateButton", "State Button");
            DrawSectionField("_upButton", "Up Button");
            DrawSectionField("_downButton", "Down Button");
        }

        DrawSpace();
        DrawSpace();
        DrawSectionHeader("Preview States");

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Normal"))
        {
            if (controller.m_normal != null)
                controller.m_normal.ApplyTransformDataInstant(controller);
            else
                Debug.LogWarning("Normal State ScriptableObject is not assigned!");
        }

        if (GUILayout.Button("Maximize"))
        {
            if (controller.m_maximized != null)
                controller.m_maximized.ApplyTransformDataInstant(controller);
            else
                Debug.LogWarning("Maximized State ScriptableObject is not assigned!");
        }

        EditorGUILayout.EndHorizontal();

        DrawSpace();
        DrawSectionHeader("Save States");

        if (GUILayout.Button("Save Normal State"))
        {
            if (controller.m_normal != null)
            {
                controller.m_normal.SaveState(controller);
                EditorUtility.SetDirty(controller.m_normal); // Mark the ScriptableObject as dirty to save changes.
            }
            else
                Debug.LogWarning("Normal State ScriptableObject is not assigned!");
        }

        if (GUILayout.Button("Save Maximized State"))
        {
            if (controller.m_maximized != null)
            {
                controller.m_maximized.SaveState(controller);
                EditorUtility.SetDirty(controller.m_maximized);
            }
            else
                Debug.LogWarning("Maximized State ScriptableObject is not assigned!");
        }

        serializedObject.ApplyModifiedProperties();
    }
}
