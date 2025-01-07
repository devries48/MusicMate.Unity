using UnityEditor;

[CustomEditor(typeof(ButtonInteractable))]
public class ButtonInteractableEditor : MusicMateEditorBase
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var button = (ButtonInteractable)target;
        //var interactableProperty = serializedObject.FindProperty("m_Interactable");

        //DrawSectionField(interactableProperty, "Interactable");

        //EditorGUILayout.PropertyField(interactableProperty);
        button.Colors = (ColorSettings)EditorGUILayout.ObjectField("Colors", button.Colors, typeof(ColorSettings), false);

        if (button.Colors != null)
        {
            DrawSpace();
            DrawSectionHeader("Preview Colors");

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ColorField("Accent", button.Colors.AccentColor);
            EditorGUILayout.ColorField("Accent Text", button.Colors.AccentTextColor);
            EditorGUILayout.ColorField("Default", button.Colors.DefaultColor);
            EditorGUILayout.ColorField("Default Text", button.Colors.TextColor);
            EditorGUILayout.ColorField("Background", button.Colors.BackgroundColor);
            EditorGUI.EndDisabledGroup();
        }

        serializedObject.ApplyModifiedProperties();
    }
}