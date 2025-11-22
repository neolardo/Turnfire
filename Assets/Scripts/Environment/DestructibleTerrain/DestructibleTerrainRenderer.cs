using System;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(SpriteRenderer))]
public class DestructibleTerrainRenderer : MonoBehaviour
{
    [SerializeField] private int _pixelsPerTile = 32;
    [SerializeField] private int _pixelsPerUnit = 64;

    private SpriteRenderer _renderer;
    private int _width, _height;
    public Texture2D Texture { get; private set; }

    public Vector2 CenteredPivotOffset { get; private set; }

    private Vector2 _textureOffset;

    private const float OverlapCheckAngleStep = 45;
    private const int MaxSearchRadiusForNormalCalculation = 4;

    private readonly Vector2Int[] NormalNeighbors = {
        new Vector2Int(-1,  0),
        new Vector2Int( 1,  0),
        new Vector2Int( 0, -1),
        new Vector2Int( 0,  1),
        new Vector2Int(-1, -1),
        new Vector2Int(-1,  1),
        new Vector2Int( 1, -1),
        new Vector2Int( 1,  1)
    };

    private readonly Vector2Int[] EdgeNeighbors =
    {
       new Vector2Int( 1, 0 ),
       new Vector2Int( -1, 0 ),
       new Vector2Int( 0, 1 ),
       new Vector2Int( 0, -1 )
    };

    #region Initialize

    public void InitializeFromTilemap(Tilemap tilemap, TilemapRenderer tilemapRenderer)
    {
        _renderer = GetComponent<SpriteRenderer>();
        Texture = BakeTilemapToTexture(tilemap);
        _width = Texture.width;
        _height = Texture.height;
        CenteredPivotOffset = (tilemap.CellToWorld(tilemap.cellBounds.min) + tilemap.CellToWorld(tilemap.cellBounds.max)) / 2;
        _textureOffset = tilemap.CellToWorld(tilemap.cellBounds.min);
        _renderer.transform.position = _textureOffset;
        _renderer.sprite = Sprite.Create(Texture, new Rect(Vector2.zero, new Vector2(Texture.width, Texture.height)), Vector2.zero, _pixelsPerUnit, 0, SpriteMeshType.Tight);
        tilemapRenderer.enabled = false;
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

    #endregion

    #region Explosion

    public void ApplyExplosion(Vector2 worldPos, float radius)
    {
        Vector2 local = WorldToLocal(worldPos);

        var p = LocalPointToPixelCoordinate(local);
        int r = Mathf.RoundToInt(radius * _pixelsPerUnit);

        for (int y = -r; y <= r; y++)
        {
            for (int x = -r; x <= r; x++)
            {
                if (x * x + y * y > r * r) continue;

                int tx = p.x + x;
                int ty = p.y + y;
                if (IsPixelOutOfBounds(tx, ty)) continue;

                Texture.SetPixel(tx, ty, Color.clear);
            }
        }

        Texture.Apply();
    }

    #endregion

    #region Overlap and Bound Checks

    public bool OverlapCircle(Vector2 worldPos, float radius)
    {
        Vector2 localCenter = WorldToLocal(worldPos);
 
        float angle = 0;
        while (angle < 360)
        {
            var ringPoint = localCenter + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            var p = LocalPointToPixelCoordinate(ringPoint);
            if (IsSolidPixel(p))
            {
                return true;
            }
            angle += OverlapCheckAngleStep;
        }
        return false;
    }

    public bool OverlapPoint(Vector2 worldPos)
    {
        Vector2 local = WorldToLocal(worldPos);
        var p = LocalPointToPixelCoordinate(local);
        return IsSolidPixel(p);
    }

    public bool IsPointInsideBounds(Vector2 worldPos)
    {
        Vector2 local = WorldToLocal(worldPos);
        var p = LocalPointToPixelCoordinate(local);
        return !IsPixelOutOfBounds(p); 
    }

    #endregion

    #region Normal Calculation

    public Vector2 GetNearestNormalAtPoint(Vector2 worldPos)
    {
        Vector2 local = WorldToLocal(worldPos);
        Vector2Int origin = LocalPointToPixelCoordinate(local);

        if (!TryFindNearestSolidEdgePixel(origin, MaxSearchRadiusForNormalCalculation, out Vector2Int nearestSolidPixel))
        {
            return Vector2.up;
        }

        return ComputeNormalAtPoint(nearestSolidPixel);
    }

    private bool TryFindNearestSolidEdgePixel(Vector2Int origin, int searchRadius, out Vector2Int edgePixel)
    {
        edgePixel = origin;

        for (int r = 0; r <= searchRadius; r++)
        {
            for (int dx = -r; dx <= r; dx++)
            {
                for (int dy = -r; dy <= r; dy++)
                {
                    if (Mathf.Abs(dx) != r && Mathf.Abs(dy) != r)
                        continue; // only check the edges of the square ring

                    int px = origin.x + dx;
                    int py = origin.y + dy;

                    if (IsPixelOutOfBounds(px, py))
                        continue;

                    if (!IsSolidPixel(px, py))
                        continue;

                    if (IsEdgePixel(px, py))
                    {
                        edgePixel = new Vector2Int(px, py);
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private bool IsEdgePixel(int x, int y)
    {
        foreach (var edgeNeighbor in EdgeNeighbors)
        {
            int nx = x + edgeNeighbor.x;
            int ny = y + edgeNeighbor.y;

            // out of bounds counts as air (edge)
            if (IsPixelOutOfBounds(nx, ny))
                return true;

            if (!IsSolidPixel(nx, ny))
                return true; 
        }

        return false;
    }

    private Vector2 ComputeNormalAtPoint(Vector2Int point)
    {
        Vector2 normal = Vector2.zero;

        foreach (var normalNeighbor in NormalNeighbors)
        {
            int nx = point.x + normalNeighbor.x;
            int ny = point.y + normalNeighbor.y;

            if (IsPixelOutOfBounds(nx, ny))
                continue;

            if (!IsSolidPixel(nx, ny)) // edge
            {
                normal += new Vector2(normalNeighbor.x, normalNeighbor.y);
            }
        }

        if (Mathf.Approximately(normal.sqrMagnitude, 0))
        {
            return Vector2.up;
        }

        return normal.normalized;
    }

    #endregion

    private bool IsSolidPixel(Vector2Int p)
    {
        return Texture.GetPixel(p.x, p.y).a >= Constants.AlphaThreshold;
    }
    private bool IsSolidPixel(int px, int py)
    {
        return Texture.GetPixel(px, py).a >= Constants.AlphaThreshold;
    }

    private bool IsPixelOutOfBounds(int px, int py)
    {
        return px < 0 || py < 0 || px >= _width || py >= _height;
    }
    private bool IsPixelOutOfBounds(Vector2Int p)
    {
        return p.x < 0 || p.y < 0 || p.x >= _width|| p.y >= _height;
    }

    private Vector2 WorldToLocal(Vector2 worldPos)
    {
        return worldPos - CenteredPivotOffset;
    }

    private Vector2Int LocalPointToPixelCoordinate(Vector2 local)
    {
        return new Vector2Int(
            Mathf.RoundToInt(local.x * _pixelsPerUnit + _width / 2f),
         Mathf.RoundToInt(local.y * _pixelsPerUnit + _height / 2f));
    }
}
