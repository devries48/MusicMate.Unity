using UnityEditor;
using UnityEngine;

public abstract class MusicMateEditorBase : Editor
{
    protected Texture2D _logoTexture;
    protected GUIStyle _headerStyle;
    protected GUIStyle _sectionStyle;

    protected virtual void OnEnable()
    {
        _logoTexture = AssetDatabase.LoadAssetAtPath <Texture2D>("Assets/Art/Sprites/spr_logo.png");

        // Initialize header style
        _headerStyle = new GUIStyle
        {
            fontSize = 12,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleLeft,
            normal = new GUIStyleState { textColor = Color.white }
        };

        // Initialize section style
        _sectionStyle = new GUIStyle
        {
            padding = new RectOffset(10, 10, 5, 5),
            normal = new GUIStyleState { background = MakeTexture(1, 1, new Color(0.2f, 0.2f, 0.2f, 0.8f)) }
        };
    }

    protected Texture2D MakeTexture(int width, int height, Color color)
    {
        var texture = new Texture2D(width, height);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                texture.SetPixel(x, y, color);
            }
        }
        texture.Apply();
        return texture;
    }

    protected virtual void DrawLogo()
    {
        if (_logoTexture != null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(_logoTexture, GUILayout.Width(200), GUILayout.Height(50));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }

    protected virtual void DrawSpace()
    {
        EditorGUILayout.Space();
    }

    protected virtual void DrawSectionHeader(string label)
    {
        GUILayout.Label(label, _headerStyle);
    }

    protected virtual void DrawSectionField(SerializedProperty property, string label, bool condition = true)
    {
        if (!condition) return; // Skip if condition is false
        GUILayout.BeginVertical();
        EditorGUILayout.PropertyField(property, new GUIContent(label));
        GUILayout.EndVertical();
    }
}
