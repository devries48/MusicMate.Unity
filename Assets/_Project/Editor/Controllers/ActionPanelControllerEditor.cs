using UnityEditor;

[CustomEditor(typeof(ActionPanelController))]
public class ActionPanelControllerEditor : MusicMateEditorBase
{
    public override void OnInspectorGUI()
    {
        var _type = serializedObject.FindProperty("_type");

        serializedObject.Update();

        DrawLogo();
        DrawTitle("Action Panel Controller");
        DrawSectionHeader("Action Panel");
        DrawSectionField(_type, "Type");

        DrawSpace();
        DrawSectionHeader("Buttons");

        var type = (ActionPanelType)_type.enumValueIndex;
        DrawSectionField("_playPauseButton", "Play & Pause");
        DrawSectionField("_playlistButton", "Add to Playlist");
        
        if (type== ActionPanelType.Release)
            DrawSectionField("_showReleaseButton", "Show Realease");

        serializedObject.ApplyModifiedProperties();
    }
}
