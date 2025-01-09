using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ShowReleaseController))]
public class PanelStateManagerEditor : MusicMateEditorBase
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        DrawSpace();

        var controller = (ShowReleaseController)target;

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
                controller.m_normal.SaveState(controller);
                EditorUtility.SetDirty(controller.m_maximized); 
            }
            else
                Debug.LogWarning("Maximized State ScriptableObject is not assigned!");
        }
    }
}
