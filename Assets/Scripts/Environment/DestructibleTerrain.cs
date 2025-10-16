using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(CompositeCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class DestructibleTerrain : MonoBehaviour
{
    [SerializeField] private DestructibleIsland _islandPrefab;
    [SerializeField] private int _pixelsPerUnit = 64;

    [SerializeField] private Texture2D _sourceTexture;
    private Renderer _sourceRenderer;
    private Color[] _pixels;
    private int _width, _height;
    private bool[,] _solid;

    [HideInInspector] public List<DestructibleIsland> Islands;

    public Vector2Int _centerPixelPosition;

    private void Awake()
    {
        Islands = new List<DestructibleIsland>();
    }

    public void InitializeFromTexture(Texture2D texture, Renderer renderer)
    {
        _sourceTexture = texture;
        _sourceRenderer = renderer;
        _width = _sourceTexture.width;
        _height = _sourceTexture.height;
        _pixels = _sourceTexture.GetPixels();
        _solid = new bool[_width, _height];

        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                _solid[x, y] = _pixels[y * _width + x].a > Constants.AlphaThreshold;
            }
        }
        _centerPixelPosition = new Vector2Int(_width /2, _height /2);

        BuildIslands();
    }


    #region Islands

    private void BuildIslands()
    {
        ClearIslands();

        bool[,] visited = new bool[_width, _height];
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                if (_solid[x, y] && !visited[x, y])
                {
                    List<Vector2Int> pixels = FloodFill(x, y, visited);
                    CreateIslandFromPixels(pixels);
                }
            }
        }
    }

    private void ClearIslands()
    {
        foreach (var isl in Islands)
        {
            if (isl != null)
            {
                Destroy(isl.gameObject);
            }
        }
        Islands.Clear();
    }

    private List<Vector2Int> FloodFill(int sx, int sy, bool[,] visited)
    {
        Queue<Vector2Int> queue = new();
        List<Vector2Int> pixels = new();

        queue.Enqueue(new Vector2Int(sx, sy));
        visited[sx, sy] = true;

        while (queue.Count > 0)
        {
            var p = queue.Dequeue();
            pixels.Add(p);

            foreach (var d in new Vector2Int[] { new(1, 0), new(-1, 0), new(0, 1), new(0, -1) })
            {
                int nx = p.x + d.x, ny = p.y + d.y;
                if (nx < 0 || ny < 0 || nx >= _width || ny >= _height) continue;
                if (!_solid[nx, ny] || visited[nx, ny]) continue;

                visited[nx, ny] = true;
                queue.Enqueue(new Vector2Int(nx, ny));
            }
        }

        return pixels;
    }

    public DestructibleIsland CreateIslandFromPixels(List<Vector2Int> pixels)
    {
        int minX = int.MaxValue, maxX = int.MinValue;
        int minY = int.MaxValue, maxY = int.MinValue;
        foreach (var p in pixels)
        {
            if (p.x < minX) minX = p.x;
            if (p.x > maxX) maxX = p.x;
            if (p.y < minY) minY = p.y;
            if (p.y > maxY) maxY = p.y;
        }

        int islandWidth = maxX - minX + 1;
        int islandHeight = maxY - minY + 1;
        Vector2 islandCenterPosition =new Vector2(minX + islandWidth / 2f, minY + islandHeight / 2f);

        Texture2D tex = new(islandWidth, islandHeight, TextureFormat.RGBA32, false);
        Color[] region = new Color[islandWidth * islandHeight];
        for (int y = 0; y < islandHeight; y++)
        {
            for (int x = 0; x < islandWidth; x++)
            {
                int sx = minX + x;
                int sy = minY + y;
                region[y * islandWidth + x] = _sourceTexture.GetPixel(sx, sy);
            }
        }
        tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.SetPixels(region);
        tex.Apply();

        var isl = Instantiate(_islandPrefab);
        isl.name = $"Island_{Islands.Count}";
        isl.transform.SetParent(transform, false);
        isl.transform.localPosition = (islandCenterPosition - _centerPixelPosition) / _pixelsPerUnit;

        var sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height),
         new Vector2(0.5f, 0.5f), _pixelsPerUnit, 0, SpriteMeshType.Tight);

        isl.Initialize(this, _sourceRenderer, sprite, new RectInt(minX, minY, islandWidth, islandHeight));

        Islands.Add(isl);

        return isl;
    }

    #endregion

    public void ApplyExplosionOld(Vector2 worldPos, float radius)
    {
        Vector2 local = transform.InverseTransformPoint(worldPos);
        float ppu = _pixelsPerUnit;

        int px = Mathf.RoundToInt(local.x * ppu + _width / 2f);
        int py = Mathf.RoundToInt(local.y * ppu + _height / 2f);
        int r = Mathf.RoundToInt(radius * ppu);

        for (int y = -r; y <= r; y++)
            for (int x = -r; x <= r; x++)
            {
                if (x * x + y * y > r * r) continue;

                int tx = px + x;
                int ty = py + y;
                if (tx < 0 || ty < 0 || tx >= _width || ty >= _height) continue;

                _sourceTexture.SetPixel(tx, ty, Color.clear);
                _solid[tx, ty] = false;
            }

        _sourceTexture.Apply();

        //List<DestructibleIsland> affected = new();
        //foreach (var isl in Islands)
        //{
        //    if (isl.Bounds.Overlaps(new RectInt(px - r, py - r, r * 2, r * 2)))
        //        affected.Add(isl);
        //}

        //foreach (var isl in affected)
        //{
        //    isl.RebuildFromTexture(_solid, Islands);
        //}

    }

    public void ApplyExplosion(Vector2 worldPos, float radius)
    {
        ApplyExplosionsOnIslands(worldPos, radius);
    }

    public void ApplyExplosionsOnIslands(Vector2 worldPos, float radius)
    {
        foreach(var island in Islands)
        {
            island.ApplyExplosion(worldPos, radius);
        }
    }
}
