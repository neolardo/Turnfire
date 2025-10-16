using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
[RequireComponent(typeof(TilemapRenderer))]
public class TilemapToDestructibleTerrainConverter : MonoBehaviour
{
    [SerializeField] private DestructibleTerrain _destructibleTerrainPrefab;
    [SerializeField] private int _pixelsPerTile = 32;

    private void Start()
    {
        ConvertTilemapToDestructibleTerrain();
    }

    private void ConvertTilemapToDestructibleTerrain()
    {
        var tilemap = GetComponent<Tilemap>();
        var tilemapRenderer = GetComponent<TilemapRenderer>();

        Texture2D texture = BakeTilemapToTexture(tilemap);

        var bounds = tilemap.cellBounds;
        Vector3Int centerCell = new Vector3Int((bounds.xMin + bounds.xMax) / 2, (bounds.yMin + bounds.yMax) / 2, 0);
        Vector3 tilemapOffset = tilemap.CellToWorld(centerCell) +  Vector3.up * (tilemap.layoutGrid.cellSize.y / 2);

        var terrain = Instantiate(_destructibleTerrainPrefab, tilemapOffset, Quaternion.identity, transform.parent);
        terrain.name = nameof(DestructibleTerrain);
        terrain.InitializeFromTexture(texture, tilemapRenderer);

        // disable tilemap
        tilemapRenderer.enabled = false;
        tilemap.gameObject.SetActive(false);
    }

    private Texture2D GetReadableTexture(Sprite sprite)
    {
        var src = sprite.texture;

        RenderTexture tmp = RenderTexture.GetTemporary(
            src.width, src.height, 0,
            RenderTextureFormat.Default, RenderTextureReadWrite.sRGB
        );

        Graphics.Blit(src, tmp);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = tmp;

        Texture2D readableTex = new Texture2D(src.width, src.height, TextureFormat.RGBA32, false, false);
        readableTex.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
        readableTex.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(tmp);

        return readableTex;
    }

    private Texture2D BakeTilemapToTexture(Tilemap tilemap)
    {
        var bounds = tilemap.cellBounds;

        var texture = new Texture2D(bounds.size.x * _pixelsPerTile, bounds.size.y * _pixelsPerTile, TextureFormat.RGBA32, false);
        var clear = new Color[texture.width * texture.height];
        texture.SetPixels(clear);

        foreach (var pos in bounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(pos);
            if (tile == null) continue;

            Sprite sprite = tilemap.GetSprite(pos);
            if (sprite == null) continue;

            var rect = sprite.textureRect;
            Texture2D readableTex = GetReadableTexture(sprite);
            var spritePixels = readableTex.GetPixels(
                (int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height
            );

            int px = (pos.x - bounds.xMin) * _pixelsPerTile;
            int py = (pos.y - bounds.yMin) * _pixelsPerTile;
            texture.SetPixels(px, py, (int)rect.width, (int)rect.height, spritePixels);
        }

        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();
        return texture;
    }


    public static Vector3 GetTilemapVisibleCenter(Tilemap tilemap)
    {
        if (tilemap == null)
        {
            Debug.LogWarning("Tilemap is null.");
            return Vector3.zero;
        }

        // Local bounds of all used tiles (in cell coordinates)
        var cellBounds = tilemap.cellBounds;

        bool hasAnyTiles = false;
        BoundsInt usedBounds = new BoundsInt();

        foreach (var pos in cellBounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                if (!hasAnyTiles)
                {
                    usedBounds = new BoundsInt(pos, Vector3Int.one);
                    hasAnyTiles = true;
                }
                else
                {
                    usedBounds.xMin = Mathf.Min(usedBounds.xMin, pos.x);
                    usedBounds.yMin = Mathf.Min(usedBounds.yMin, pos.y);
                    usedBounds.xMax = Mathf.Max(usedBounds.xMax, pos.x + 1);
                    usedBounds.yMax = Mathf.Max(usedBounds.yMax, pos.y + 1);
                }
            }
        }

        if (!hasAnyTiles)
        {
            Debug.LogWarning("Tilemap has no tiles!");
            return  Vector3.zero;
        }

        // Convert bounds from cell space to world space
        Vector3 minWorld = tilemap.CellToWorld(new Vector3Int(usedBounds.xMin, usedBounds.yMin, 0));
        Vector3 maxWorld = tilemap.CellToWorld(new Vector3Int(usedBounds.xMax, usedBounds.yMax, 0));

        // Calculate world-space bounds
        Bounds worldBounds = new Bounds();
        worldBounds.SetMinMax(minWorld, maxWorld);

        Vector3 worldCenter = worldBounds.center;

        return worldCenter;
    }
}
