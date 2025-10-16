using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
[RequireComponent(typeof(TilemapCollider2D))]
[RequireComponent(typeof(TilemapRenderer))]
public class TilemapToDestructibleTerrainConverter : MonoBehaviour
{
    [SerializeField] private Vector2 _textureOffset = new Vector2(1, 0.25f);
    [SerializeField] private DestructibleTerrain _destructibleTerrainPrefab;

    private void Awake()
    {
        ConvertTilemapToPolygons();
    }

    private void ConvertTilemapToPolygons()
    {
        var tilemap = GetComponent<Tilemap>();
        var tilemapCollider = GetComponent<TilemapCollider2D>();
        var tilemapRenderer = GetComponent<TilemapRenderer>();

        var destructibleTerrain = Instantiate(_destructibleTerrainPrefab, _textureOffset, Quaternion.identity, transform.parent);
        destructibleTerrain.transform.SetParent(transform.parent);
        destructibleTerrain.name = nameof(DestructibleTerrain);

        ConvertTilemapColliderToPolygonColliders(tilemapCollider, destructibleTerrain.transform);
        var convertedSprite = ConvertTilemapToSprite(tilemap, tilemapRenderer);

        destructibleTerrain.Initialize(convertedSprite);

        tilemapRenderer.enabled = false;
        tilemapCollider.enabled = false;
        tilemapRenderer.gameObject.SetActive(false);  
    }

    private void ConvertTilemapColliderToPolygonColliders(TilemapCollider2D collider, Transform compositeColliderParent)
    {
        // Force collider to update
        collider.ProcessTilemapChanges();

        // Access composite collider data if available
        var composite = GetComponent<CompositeCollider2D>();
        if (composite != null)
        {
            int pathCount = composite.pathCount;

            for (int i = 0; i < pathCount; i++)
            {
                var points = new List<Vector2>();
                composite.GetPath(i, points);

                var go = new GameObject($"Polygon_{i}");
                go.transform.SetParent(compositeColliderParent);
                go.transform.position = Vector3.zero;

                var poly = go.AddComponent<PolygonCollider2D>();
                poly.points = points.ToArray();
                poly.compositeOperation = Collider2D.CompositeOperation.Merge;
            }
        }
        else
        {
            // If no composite, use tilemap collider directly
            var bounds = collider.bounds;
            var go = new GameObject($"Polygon_FromTilemap");
            go.transform.SetParent(compositeColliderParent);
            go.transform.position = Vector3.zero;

            var poly = go.AddComponent<PolygonCollider2D>();
            poly.points = new Vector2[]
            {
                new(bounds.min.x, bounds.min.y),
                new(bounds.max.x, bounds.min.y),
                new(bounds.max.x, bounds.max.y),
                new(bounds.min.x, bounds.max.y)
            };
        }
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


    private Sprite ConvertTilemapToSprite(Tilemap tilemap, TilemapRenderer renderer)
    {
        // Create a Texture2D of the tilemap’s visual
        var bounds = tilemap.cellBounds;
        int ppu = 32;
        var texture = new Texture2D(bounds.size.x * ppu, bounds.size.y * ppu, TextureFormat.RGBA32, false);
        var pixels = new Color[texture.width * texture.height];

        // Fill background as transparent
        texture.SetPixels(0, 0, texture.width, texture.height, pixels);

        foreach (var pos in bounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(pos);
            if (tile == null) continue;

            Sprite sprite = tilemap.GetSprite(pos);
            if (sprite == null) continue;

            // Copy sprite pixels into our texture
            var rect = sprite.textureRect;
            Texture2D readableTex = GetReadableTexture(sprite);
            var spritePixels = readableTex.GetPixels(
                (int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height
            );
            int x = (pos.x - bounds.xMin) * ppu;
            int y = (pos.y - bounds.yMin) * ppu;

            texture.SetPixels(x, y, (int)rect.width, (int)rect.height, spritePixels);
        }

        texture.Apply();

        // Create sprite
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 64);

    }
}
