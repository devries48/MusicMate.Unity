using UnityEditor;

[CustomEditor(typeof(ButtonInteractable))]
public class ButtonInteractableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        ButtonInteractable button = (ButtonInteractable)target;
        SerializedProperty interactableProperty = serializedObject.FindProperty("m_Interactable");

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(interactableProperty);
        button.Colors = (ColorSettings)EditorGUILayout.ObjectField("Colors", button.Colors, typeof(ColorSettings), false);

        if (button.Colors != null)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Preview Colors", EditorStyles.boldLabel);

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