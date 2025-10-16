using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
public class SimpleDestructibleTerrain : MonoBehaviour
{
    [Range(0f, 1f)]
    public float alphaThreshold = 0.1f;

    private Texture2D _texture;
    private SpriteRenderer _renderer;
    private PolygonCollider2D _collider;

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<PolygonCollider2D>();

        Texture2D source = _renderer.sprite.texture;
        _texture = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);
        _texture.filterMode = source.filterMode;
        _texture.wrapMode = source.wrapMode;
        _texture.SetPixels(source.GetPixels());
        _texture.Apply();

        _renderer.sprite = Sprite.Create(
            _texture,
            _renderer.sprite.rect,
            new Vector2(0.5f, 0.5f),
            _renderer.sprite.pixelsPerUnit,
            0,
            SpriteMeshType.Tight
        );

        //RebuildCollider();
    }

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

    private void RebuildCollider()
    {
        if (_collider != null) Destroy(_collider);
        _collider = gameObject.AddComponent<PolygonCollider2D>();


        // Step 1: Get edge pixels
        List<Vector2> edgePoints = GetEdgePixels(_renderer.sprite);

        // Step 2: Order points along the contour
        List<Vector2> ordered = OrderContourPoints(edgePoints);

        // Step 3: Remove collinear points
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
                if (pixels[idx].a <= alphaThreshold) continue;

                bool isEdge = false;
                for (int ny = -1; ny <= 1; ny++)
                {
                    for (int nx = -1; nx <= 1; nx++)
                    {
                        if (nx == 0 && ny == 0) continue;
                        int cx = x + nx;
                        int cy = y + ny;
                        if (cx < 0 || cy < 0 || cx >= width || cy >= height || pixels[cy * width + cx].a <= alphaThreshold)
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


}
