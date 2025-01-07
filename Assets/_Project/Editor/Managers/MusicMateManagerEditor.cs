using UnityEditor;

[CustomEditor(typeof(MusicMateManager))]
public class MusicMateManagerEditor : MusicMateEditorBase
{
    protected override void OnEnable()
    {
        base.OnEnable();
        InitializeFoldoutState("Windows");
        InitializeFoldoutState("Animators");
        InitializeFoldoutState("Elements");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawLogo();
        DrawTitle("MusicMate Manager");
        DrawSpace();
        DrawDescription("Handles application-wide configuration and high-level events.");
        DrawSpace();

        // Display a warning if the object is not in the root
        var manager = (MusicMateManager)target;
        if (manager.transform.parent != null)
        {
            EditorGUILayout.HelpBox("Warning: This manager should be at the root level of the hierarchy.", MessageType.Warning);
        }

        DrawSectionField(serializedObject.FindProperty("_appConfig"), "App Configuration");
        DrawSpace();

        if (DrawFoldout("Windows"))
        {
            DrawSectionField(serializedObject.FindProperty("_errorController"), "Error Window");
            DrawSectionField(serializedObject.FindProperty("_loginController"), "Login Window");
        }

        if (DrawFoldout("Animators"))
        {
            DrawSectionField(serializedObject.FindProperty("_mainPage"), "Main Page Animator");
            DrawSectionField(serializedObject.FindProperty("_logoAnimator"), "Logo Animator");
        }

        if (DrawFoldout("Elements"))
        {
            DrawSectionField(serializedObject.FindProperty("_connectionSpinner"), "Connection Spinner");
            DrawSectionField(serializedObject.FindProperty("_activateOnStart"), "Activate On Start");
            DrawSectionField(serializedObject.FindProperty("_inactivateOnStart"), "Inactivate On Start");
        }

        serializedObject.ApplyModifiedProperties();
    }
}
