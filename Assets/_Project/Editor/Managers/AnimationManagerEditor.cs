using UnityEditor;

[CustomEditor(typeof(AnimationManager))]
public class AnimationManagerEditor : MusicMateEditorBase
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawTitle("Animation Manager");
        DrawDescription("The Animation Manager serves as the central hub for managing and orchestrating animations across various UI elements and components in the application.", false);
        DrawSpace();

        DrawWarningIfNotInRoot(((AnimationManager)target).transform);
        DrawDefaultFields();

        serializedObject.ApplyModifiedProperties();
    }
}
