using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class MusicMateEditorBase : Editor
{
    protected Texture2D _logoTexture;
    readonly Dictionary<string, bool> _foldoutStates = new();
    static readonly string[] _dontIncludeMe = new string[] { "m_Script" };

    protected virtual void OnEnable() => _logoTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Art/Sprites/spr_logo.png");

    protected virtual void DrawDefaultFields()
    {
        DrawPropertiesExcluding(serializedObject, _dontIncludeMe);
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

    protected virtual void DrawTitle(string title)
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label(title, CustomEditorStyles.titleStyle, GUILayout.Width(EditorGUIUtility.currentViewWidth - 40));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        DrawSpace();
    }

    protected virtual void DrawDescription(string description, bool drawSpace = true)
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label(description, CustomEditorStyles.descriptionStyle, GUILayout.Width(EditorGUIUtility.currentViewWidth - 32));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        if (drawSpace)
            DrawSpace();
    }

    protected virtual void DrawWarningIfNotInRoot(Transform transform)
    {
        if (transform.parent != null)
            EditorGUILayout.HelpBox("Warning: This manager should be at the root level of the hierarchy.", MessageType.Warning);
    }

    protected virtual void DrawSpace() => EditorGUILayout.Space();

    protected virtual void DrawSectionHeader(string label) => GUILayout.Label(label, CustomEditorStyles.headerStyle);

    protected virtual void DrawSectionField(SerializedProperty property, string label, bool condition = true)
    {
        if (!condition) return;
        GUILayout.BeginVertical();
        EditorGUILayout.PropertyField(property, new GUIContent(label));
        GUILayout.EndVertical();
    }

    protected virtual void DrawSectionField(string propertyName, string label, bool condition = true)
    {
        if (!condition) return;

        var property = Find(propertyName);
        
        GUILayout.BeginVertical();
        EditorGUILayout.PropertyField(property, new GUIContent(label));
        GUILayout.EndVertical();
    }

    protected void InitializeFoldoutState(string sectionName)
    {
        if (!_foldoutStates.ContainsKey(sectionName))
        {
            bool defaultState = EditorPrefs.GetBool(GetFoldoutKey(sectionName), false); // Default to folded
            _foldoutStates[sectionName] = defaultState;
        }
    }

    protected bool DrawFoldout(string label)
    {
        _foldoutStates[label] = EditorGUILayout.Foldout(_foldoutStates[label], label, true, CustomEditorStyles.foldoutStyle);
        EditorPrefs.SetBool(GetFoldoutKey(label), _foldoutStates[label]);
        return _foldoutStates[label];
    }

    SerializedProperty Find(string name) => serializedObject.FindProperty(name);

    string GetFoldoutKey(string sectionName) => $"{target.GetType().Name}_{sectionName}_Foldout";
}

public static class CustomEditorStyles
{
    public static GUIStyle customStyle;
    public static GUIStyle headerStyle, sectionStyle, foldoutStyle, titleStyle, descriptionStyle;

    static CustomEditorStyles()
    {
        ColorUtility.TryParseHtmlString("#9F9F9F", out Color foldoutColor);
        ColorUtility.TryParseHtmlString("#F7B448", out Color titleColor);
        ColorUtility.TryParseHtmlString("#FAF9F6", out Color descriptionColor);

        headerStyle = new GUIStyle
        {
            fontSize = 12,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleLeft,
            normal = new GUIStyleState { textColor = Color.white }
        };

        sectionStyle = new GUIStyle
        {
            padding = new RectOffset(10, 10, 5, 5),
            normal = new GUIStyleState { background = MakeTexture(1, 1, new Color(0.2f, 0.2f, 0.2f, 0.8f)) }
        };

        foldoutStyle = new GUIStyle(EditorStyles.foldout)
        {
            fontStyle = FontStyle.Bold,
            normal = { textColor = foldoutColor },
            onNormal = { textColor = foldoutColor },
            focused = { textColor = Color.white },
            onFocused = { textColor = Color.white },
            active = { textColor = foldoutColor },
            onActive = { textColor = foldoutColor }
        };

        titleStyle = new GUIStyle
        {
            fontSize = 12,
            fontStyle = FontStyle.Normal,
            alignment = TextAnchor.MiddleCenter,
            wordWrap = true,
            normal = new GUIStyleState { textColor = titleColor }
        };

        descriptionStyle = new GUIStyle
        {
            fontSize = 12,
            fontStyle = FontStyle.Normal,
            alignment = TextAnchor.LowerLeft,
            wordWrap = true,
            normal = new GUIStyleState { textColor = descriptionColor }
        };

    }

    static Texture2D MakeTexture(int width, int height, Color color)
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

}
