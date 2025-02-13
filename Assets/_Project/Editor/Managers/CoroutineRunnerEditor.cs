using UnityEditor;

[CustomEditor(typeof(CoroutineRunner))]
public class CoroutineRunnerEditor : MusicMateEditorBase
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawTitle("Coroutine Runner");
        DrawDescription("Run coroutines globally, even if the calling object is inactive.", false);
        DrawSpace();

        serializedObject.ApplyModifiedProperties();
    }
}
