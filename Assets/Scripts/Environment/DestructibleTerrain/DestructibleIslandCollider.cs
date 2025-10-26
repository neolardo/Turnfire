using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class DestructibleIslandCollider : MonoBehaviour
{
    private PolygonCollider2D _collider;
    private int _pixelsPerUnit;
    private List<Vector2> _path;

    private static readonly Vector2Int[] TraceDirections =  new Vector2Int[]
    {
        new Vector2Int(1, 0),   // Right
        new Vector2Int(1, -1),  // Down-right
        new Vector2Int(0, -1),  // Down
        new Vector2Int(-1, -1), // Down-left
        new Vector2Int(-1, 0),  // Left
        new Vector2Int(-1, 1),  // Up-left
        new Vector2Int(0, 1),   // Up
        new Vector2Int(1, 1)    // Up-right
    };

    private void Awake()
    {
        _collider = GetComponent<PolygonCollider2D>();
        _path = new List<Vector2>();
    }

    public IEnumerator RebuildColliderFromPixelMaskAsync(PixelMask islandPixelMask, int pixelsPerUnit)
    {
        _collider.enabled = false;
        _path.Clear();
        _pixelsPerUnit = pixelsPerUnit;
        yield return TraceAsync(islandPixelMask);
        yield return RemoveCollinearPointsAsync();
        _collider.pathCount = 1;
        _collider.SetPath(0, _path);
        _collider.enabled = true;
    }


    public IEnumerator TraceAsync(PixelMask mask, int yieldInterval = 2000)
    {
        bool[,] grid = mask.Mask;
        int width = mask.Width;
        int height = mask.Height;

        Vector2Int start = FindStartPixel(grid, width, height);
        if (start.x == -1)
        {
            yield break;
        }

        Vector2Int current = start;
        int dir = 0; // direction index in clockwise order

        _path.Add(ToLocal(current, mask.Rect, _pixelsPerUnit));

        bool finished = false;
        int iteration = 0;
        do
        {
            int startDir = (dir + 6) % 8; // turn left relative to entry direction
            bool foundNext = false;

            for (int i = 0; i < 8; i++)
            {
                int checkDir = (startDir + i) % TraceDirections.Length;
                Vector2Int next = current + TraceDirections[checkDir];
                if (Inside(next, width, height) && grid[next.x, next.y])
                {
                    current = next;
                    dir = checkDir;
                    _path.Add(ToLocal(current, mask.Rect, _pixelsPerUnit));
                    foundNext = true;
                    break;
                }
            }

            if (!foundNext)
                break;

            if (current == start && _path.Count > 3)
                finished = true;

            iteration++;
            if (iteration % yieldInterval == 0)
            {
                yield return null;
            }
        }
        while (!finished);

    }

    private static Vector2Int FindStartPixel(bool[,] grid, int width, int height)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (grid[x, y])
                {
                    if (IsBoundary(grid, width, height, x, y))
                        return new Vector2Int(x, y);
                }
            }
        }
        return new Vector2Int(-1, -1);
    }

    private static bool IsBoundary(bool[,] grid, int width, int height, int x, int y)
    {
        if (!grid[x, y]) return false;
        for (int ny = -1; ny <= 1; ny++)
        {
            for (int nx = -1; nx <= 1; nx++)
            {
                int cx = x + nx;
                int cy = y + ny;
                if (cx < 0 || cy < 0 || cx >= width || cy >= height)
                    return true; // touches border
                if (!grid[cx, cy])
                    return true; // neighbor is empty
            }
        }
        return false;
    }

    private static bool Inside(Vector2Int p, int w, int h)
    {
        return p.x >= 0 && p.y >= 0 && p.x < w && p.y < h;
    }

    private Vector2 ToLocal(Vector2Int point, RectInt rect, float ppu)
    {
        return new Vector2(
            (point.x - rect.width * 0.5f) / ppu,
            (point.y - rect.height * 0.5f) / ppu
        );
    }

    private IEnumerator RemoveCollinearPointsAsync(float tolerance = 0.001f, int yieldInterval = 2000)
    {
        if (_path == null || _path.Count < 3)
            yield break;

        var result = new List<Vector2>();
        int n = _path.Count;

        for (int i = 0; i < n; i++)
        {
            Vector2 prev = _path[(i - 1 + n) % n];
            Vector2 curr = _path[i];
            Vector2 next = _path[(i + 1) % n];

            Vector2 dir1 = (curr - prev).normalized;
            Vector2 dir2 = (next - curr).normalized;

            // check if they're nearly the same direction
            float dot = Vector2.Dot(dir1, dir2);

            // If directions are almost perfectly aligned to collinear
            if (Mathf.Abs(dot - 1f) < tolerance)
                continue;

            // Otherwise, keep the point
            result.Add(curr);

            if(n % yieldInterval == 0)
            {
                yield return null;
            }
        }

        _path = result;
    }

}