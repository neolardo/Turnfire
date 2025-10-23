using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class MinimapBaker : MonoBehaviour
{
    [SerializeField] private Tilemap _groundTilemap;
    [SerializeField] private Tilemap _waterTilemap;
    [SerializeField] private Team[] _teams;
    [SerializeField] private Vector2Int _targetSize = new Vector2Int(60, 32);
    [SerializeField] private Color _groundColor = new Color(0.4f, 0.25f, 0.1f);
    [SerializeField] private Color _waterColor = new Color(0, 0.2f, .6f);

     private Texture2D bakedMinimap;

    public Texture2D BakedMinimap => bakedMinimap;

    [ContextMenu("Bake Minimap")]
    public void BakeMinimap()
    {
        if (_groundTilemap == null)
        {
            Debug.LogError("[MinimapBaker] Ground Tilemap not assigned.");
            return;
        }

        Debug.Log("[MinimapBaker] Baking minimap...");

        var bounds = _groundTilemap.cellBounds;
        Vector2Int offset = new Vector2Int(bounds.xMin, bounds.yMin);

        var tex = new Texture2D(_targetSize.x, _targetSize.y, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;

        // clear texture
        Color[] clear = new Color[_targetSize.x * _targetSize.y];
        for (int i = 0; i < clear.Length; i++) clear[i] = Color.clear;
        tex.SetPixels(clear);

        Vector2 scale = new Vector2(
            (float)_targetSize.x / bounds.size.x,
            (float)_targetSize.y / bounds.size.y
        );

        // daint ground and water
        foreach (var pos in bounds.allPositionsWithin)
        {
            bool hasGround = _groundTilemap.HasTile(pos);
            bool hasWater = _waterTilemap != null && _waterTilemap.HasTile(pos);

            if (!hasGround && !hasWater)
                continue;

            float xStart = (pos.x - offset.x) * scale.x;
            float yStart = (pos.y - offset.y) * scale.y;
            float xEnd = xStart + scale.x;
            float yEnd = yStart + scale.y;

            int xMin = Mathf.Clamp(Mathf.FloorToInt(xStart), 0, _targetSize.x - 1);
            int yMin = Mathf.Clamp(Mathf.FloorToInt(yStart), 0, _targetSize.y - 1);
            int xMax = Mathf.Clamp(Mathf.CeilToInt(xEnd), 0, _targetSize.x);
            int yMax = Mathf.Clamp(Mathf.CeilToInt(yEnd), 0, _targetSize.y);

            Color color = hasGround ? _groundColor : _waterColor;

            for (int x = xMin; x < xMax; x++)
            {
                for (int y = yMin; y < yMax; y++)
                {
                    tex.SetPixel(x, y, color);
                }
            }
        }

        // draw characters 
        var characters = new List<Character>();
        foreach (var team in _teams)
        {
            characters.Clear();
            for (int i = 0; i < team.transform.childCount; i++)
            {
                characters.Add(team.transform.GetChild(i).GetComponent<Character>());
            }

            foreach (var c in characters)
            {
                var color = team.TeamColor;
                var worldPos = c.transform.position;
                var cellPos = _groundTilemap.WorldToCell(worldPos);

                int x = Mathf.FloorToInt((cellPos.x - offset.x) * scale.x);
                int y = Mathf.FloorToInt((cellPos.y - offset.y) * scale.y);

                for (int px = -1; px <= 1; px++)
                {
                    for (int py = -1; py <= 1; py++)
                    {
                        int tx = Mathf.Clamp(x + px, 0, _targetSize.x - 1);
                        int ty = Mathf.Clamp(y + py, 0, _targetSize.y - 1);
                        tex.SetPixel(tx, ty, color);
                    }
                }
            }
        }


        tex.Apply();
        bakedMinimap = tex;
        SaveAsPng(tex);

        Debug.Log("[MinimapBaker] Minimap baked and saved.");
    }

    private void SaveAsPng(Texture2D texture)
    {
        string sceneName = Path.GetFileNameWithoutExtension(EditorSceneManager.GetActiveScene().path);
        string folderPath = Constants.MinimapFolderPath;

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string baseFile = $"{sceneName}";
        string filePath = $"{folderPath}/{baseFile}.png";
        int index = 1;

        while (File.Exists(filePath))
        {
            filePath = $"{folderPath}/{baseFile}_{index}.png";
            index++;
        }

        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);

        AssetDatabase.Refresh();

        Debug.Log($"[MinimapBaker] Saved to: {filePath}");
    }

}
