using UnityEditor;

[CustomEditor(typeof(MusicMateApiService))]
public class MusicMateApiServiceEditor : MusicMateEditorBase
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawLogo();
        DrawTitle("MusicMate API Service");
        DrawDescription("This service handles user authentication, data retrieval, image downloads and folder import operations.");

        DrawWarningIfNotInRoot(((MusicMateApiService)target).transform);

        serializedObject.ApplyModifiedProperties();
    }
}
