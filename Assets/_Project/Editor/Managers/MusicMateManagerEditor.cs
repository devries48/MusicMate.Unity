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
        DrawDescription("The central manager for the MusicMate application.Handles configuration, initialization and high-level application events such as connecting to the API or showing windows.");
        DrawSpace();

        DrawWarningIfNotInRoot(((MusicMateManager)target).transform);

        DrawSectionField("_appSettings", "App Settings");
        DrawSpace();

        if (DrawFoldout("Windows"))
        {
            DrawSectionField("_errorWindow", "Error Window");
            DrawSectionField("_loginWindow", "Login Window");
            DrawSectionField("_editorWindow", "Editor Window");
        }

        if (DrawFoldout("Animators"))
        {
            DrawSectionField("_mainPage", "Main Page Animator");
            DrawSectionField("_logoAnimator", "Logo Animator");
        }

        if (DrawFoldout("Elements"))
        {
            DrawSectionField("_connectionSpinner", "Connection Spinner");
            DrawSectionField("_activateOnStart", "Activate On Start");
            DrawSectionField("_inactivateOnStart", "Inactivate On Start");
        }

        serializedObject.ApplyModifiedProperties();
    }
}
