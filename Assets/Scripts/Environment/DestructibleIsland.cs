using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(PolygonCollider2D))]
public class DestructibleIsland : MonoBehaviour
{
    private DestructibleTerrainRenderer _terrain;
    private Texture2D _texture;
    private SpriteRenderer _renderer;
    private PolygonCollider2D _collider;
    public RectInt Bounds { get; private set; }

    public void Initialize(DestructibleTerrainRenderer terrain, Renderer sourceRenderer, Sprite sprite, RectInt bounds)
    {
        _terrain = terrain;
        _texture = sprite.texture;
        _renderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<PolygonCollider2D>();

        _renderer.sprite = sprite;
        _renderer.sortingLayerID = sourceRenderer.sortingLayerID;
        _renderer.sortingLayerName = sourceRenderer.sortingLayerName;
        _renderer.sortingOrder = sourceRenderer.sortingOrder;
        Bounds = bounds;
        RebuildCollider();
    }


    #region Sprite

    public void RebuildFromTexture(bool[,] solid, List<DestructibleIsland> islandList)
    {
        RecreateSprite(solid, islandList);
        RebuildCollider();
    }


    private void RecreateSprite(bool[,] solid, List<DestructibleIsland> islandList)
    {
        List<List<Vector2Int>> newPixelIslands = new();
        bool[,] visited = new bool[Bounds.width, Bounds.height];

        for (int y = 0; y < Bounds.height; y++)
        {
            for (int x = 0; x < Bounds.width; x++)
            {
                int sx = Bounds.x + x;
                int sy = Bounds.y + y;
                if (!solid[sx, sy] || visited[x, y])
                    continue;

                var pixels = FloodFillLocal(solid, visited, sx, sy);
                newPixelIslands.Add(pixels);
            }
        }

        if (newPixelIslands.Count == 0)
        {
            // Island fully destroyed
            islandList.Remove(this);
            Destroy(gameObject);
            return;
        }

        // Reuse this island for the first set of pixels
        var first = newPixelIslands[0];
        UpdateSpriteFromPixels(first);

        // Create new ones for remaining fragments
        //for (int i = 1; i < newPixelIslands.Count; i++)
        //{
        //    _terrain.CreateIslandFromPixels(newPixelIslands[i]);
        //}
    }


    private List<Vector2Int> FloodFillLocal(bool[,] solid, bool[,] visited, int sx, int sy)
    {
        Queue<Vector2Int> q = new();
        List<Vector2Int> result = new();
        q.Enqueue(new Vector2Int(sx, sy));
        visited[sx - Bounds.x, sy - Bounds.y] = true;

        while (q.Count > 0)
        {
            var p = q.Dequeue();
            result.Add(p);
            foreach (var d in new Vector2Int[] { new(1, 0), new(-1, 0), new(0, 1), new(0, -1) })
            {
                int nx = p.x + d.x, ny = p.y + d.y;
                if (nx < Bounds.x || ny < Bounds.y || nx >= Bounds.xMax || ny >= Bounds.yMax)
                    continue;
                if (!solid[nx, ny] || visited[nx - Bounds.x, ny - Bounds.y])
                    continue;
                visited[nx - Bounds.x, ny - Bounds.y] = true;
                q.Enqueue(new Vector2Int(nx, ny));
            }
        }
        return result;
    }

    private void UpdateSpriteFromPixels(List<Vector2Int> pixels)
    {
        // Rebuild sprite texture with tight mesh
        Texture2D newTex;
        RectInt newBounds = CalculateBounds(pixels, out newTex);

        var ppu = _renderer.sprite.pixelsPerUnit;

        _renderer.sprite = Sprite.Create(newTex, new Rect(0, 0, newTex.width, newTex.height),
            new Vector2(0.5f, 0.5f), ppu, 0, SpriteMeshType.Tight);

        Bounds = newBounds;
    }

    private RectInt CalculateBounds(List<Vector2Int> pixels, out Texture2D tex)
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

        int width = maxX - minX + 1;
        int height = maxY - minY + 1;
        tex = new Texture2D(width, height, TextureFormat.RGBA32, false);

        Color[] region = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int sx = minX + x;
                int sy = minY + y;
                region[y * width + x] = _texture.GetPixel(sx, sy);
            }
        }

        tex.SetPixels(region);
        tex.Apply();
        return new RectInt(minX, minY, width, height);
    }

    #endregion

    #region Collider

    private void RebuildCollider()
    {
        List<Vector2> edgePoints = GetEdgePixels(_renderer.sprite);
        List<Vector2> ordered = OrderContourPoints(edgePoints);
        ordered = RemoveCollinearPoints(ordered);

        List<Vector2> path = ordered;
        _collider.pathCount = 1;
        _collider.SetPath(0, path);
    }

    private List<Vector2> GetEdgePixels(Sprite sprite)
    {
        List<Vector2> edges = new List<Vector2>();

        Texture2D tex = sprite.texture;
        if (!tex.isReadable)
        {
            Debug.LogError("Texture must be readable!");
            return edges;
        }

        int width = (int)sprite.rect.width;
        int height = (int)sprite.rect.height;
        Color[] pixels = tex.GetPixels(
            (int)sprite.rect.x,
            (int)sprite.rect.y,
            width,
            height
        );

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int idx = y * width + x;
                if (pixels[idx].a <= Constants.AlphaThreshold) continue;

                bool isEdge = false;
                for (int ny = -1; ny <= 1; ny++)
                {
                    for (int nx = -1; nx <= 1; nx++)
                    {
                        if (nx == 0 && ny == 0) continue;
                        int cx = x + nx;
                        int cy = y + ny;
                        if (cx < 0 || cy < 0 || cx >= width || cy >= height || pixels[cy * width + cx].a <= Constants.AlphaThreshold)
                        {
                            isEdge = true;
                            break;
                        }
                    }
                    if (isEdge) break;
                }

                if (isEdge)
                {
                    Vector2 localPos = new Vector2(
                        (x - width * 0.5f) / sprite.pixelsPerUnit,
                        (y - height * 0.5f) / sprite.pixelsPerUnit
                    );
                    edges.Add(localPos);
                }
            }
        }

        return edges;
    }

    private List<Vector2> OrderContourPoints(List<Vector2> points)
    {
        if (points.Count < 3) return points;

        List<Vector2> ordered = new List<Vector2>();
        HashSet<int> visited = new HashSet<int>();

        // Start with the first point
        int currentIndex = 0;
        ordered.Add(points[currentIndex]);
        visited.Add(currentIndex);

        while (ordered.Count < points.Count)
        {
            Vector2 current = points[currentIndex];
            float minDist = float.MaxValue;
            int nextIndex = -1;

            // Find the closest unvisited point
            for (int i = 0; i < points.Count; i++)
            {
                if (visited.Contains(i)) continue;
                float dist = Vector2.SqrMagnitude(points[i] - current);
                if (dist < minDist)
                {
                    minDist = dist;
                    nextIndex = i;
                }
            }

            if (nextIndex == -1) break; // No more points
            ordered.Add(points[nextIndex]);
            visited.Add(nextIndex);
            currentIndex = nextIndex;
        }

        return ordered;
    }

    private List<Vector2> RemoveCollinearPoints(List<Vector2> points)
    {
        if (points.Count < 3) return points;

        List<Vector2> result = new List<Vector2>();
        int n = points.Count;

        for (int i = 0; i < n; i++)
        {
            Vector2 prev = points[(i - 1 + n) % n];
            Vector2 curr = points[i];
            Vector2 next = points[(i + 1) % n];

            if (!IsCollinear(prev, curr, next))
                result.Add(curr);
        }

        return result;
    }

    private bool IsCollinear(Vector2 a, Vector2 b, Vector2 c, float epsilon = 0.0001f)
    {
        float area = Mathf.Abs((a.x * (b.y - c.y) + b.x * (c.y - a.y) + c.x * (a.y - b.y)) / 2f);
        return area < epsilon;
    }

    #endregion

    public void ApplyExplosion(Vector2 worldPos, float radius)
    {
        Vector2 localPos = transform.InverseTransformPoint(worldPos);
        Sprite sprite = _renderer.sprite;
        float ppu = sprite.pixelsPerUnit;
        Vector2 pivot = sprite.pivot;

        float spriteWorldWidth = sprite.rect.width / ppu;
        float spriteWorldHeight = sprite.rect.height / ppu;

        float unitsToPixelsX = _texture.width / (spriteWorldWidth * transform.localScale.x);
        float unitsToPixelsY = _texture.height / (spriteWorldHeight * transform.localScale.y);

        int pxCenter = Mathf.RoundToInt(localPos.x * unitsToPixelsX + pivot.x);
        int pyCenter = Mathf.RoundToInt(localPos.y * unitsToPixelsY + pivot.y);

        int rX = Mathf.RoundToInt(radius * unitsToPixelsX);
        int rY = Mathf.RoundToInt(radius * unitsToPixelsY);

        for (int x = -rX; x <= rX; x++)
        {
            for (int y = -rY; y <= rY; y++)
            {
                float normX = (float)x / rX;
                float normY = (float)y / rY;

                if (normX * normX + normY * normY <= 1f)
                {
                    int px = pxCenter + x;
                    int py = pyCenter + y;

                    if (px >= 0 && px < _texture.width && py >= 0 && py < _texture.height)
                        _texture.SetPixel(px, py, Color.clear);
                }
            }
        }

        _texture.Apply();
        RebuildCollider();
    }

}
