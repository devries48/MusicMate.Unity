using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MusicMate/Settings/Editor Prefabs", fileName = "Editor Prefab Config")]
public class EditorPrefabConfig : ScriptableObject
{
    [Serializable]
    public class EditorPrefabEntry
    {
        public MusicMateZone editorType;
        public GameObject prefab;
    }

    public List<EditorPrefabEntry> editorPrefabs = new();

    public GameObject GetPrefab(MusicMateZone editor)
    {
        var entry = editorPrefabs.Find(e => e.editorType == editor);
        return entry?.prefab;
    }
}
