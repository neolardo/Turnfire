using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MinimapBaker))]
public class MinimapBakerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var baker = (MinimapBaker)target;

        EditorGUILayout.Space(10);
        GUI.backgroundColor = Color.green;

        if (GUILayout.Button("Bake Minimap"))
        {
            baker.BakeMinimap();

            // Mark scene as dirty so changes persist
            EditorUtility.SetDirty(baker);
        }

        GUI.backgroundColor = Color.white;

        if (baker.BakedMinimap != null)
        {
            GUILayout.Label("Preview:", EditorStyles.boldLabel);
            GUILayout.Box(baker.BakedMinimap, GUILayout.Width(128), GUILayout.Height(128));
        }
    }
}