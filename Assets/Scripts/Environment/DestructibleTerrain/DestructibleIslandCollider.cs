using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class DestructibleIslandCollider : MonoBehaviour
{
    private PolygonCollider2D _collider;
    private int _pixelsPerUnit;

    private void Awake()
    {
        _collider = GetComponent<PolygonCollider2D>();
    }

    public void RebuildColliderFromPixelMask(PixelMask islandPixelMask, int pixelsPerUnit)
    {
        _pixelsPerUnit = pixelsPerUnit;
        var path = Trace(islandPixelMask);
        path = RemoveCollinearPoints(path);
        _collider.pathCount = 1;
        _collider.SetPath(0, path);
    }


    public List<Vector2> Trace(PixelMask mask)
    {
        bool[,] grid = mask.Mask;
        int width = mask.Width;
        int height = mask.Height;

        // Find the first boundary pixel
        Vector2Int start = FindStartPixel(grid, width, height);
        if (start.x == -1)
            return new List<Vector2>();

        List<Vector2> contour = new List<Vector2>();
        Vector2Int current = start;
        int dir = 0; // direction index in clockwise order

        // Directions (clockwise)
        Vector2Int[] dirs = new Vector2Int[]
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

        contour.Add(ToLocal(current, mask.Rect, _pixelsPerUnit));

        bool finished = false;

        do
        {
            int startDir = (dir + 6) % 8; // turn left relative to entry direction
            bool foundNext = false;

            for (int i = 0; i < 8; i++)
            {
                int checkDir = (startDir + i) % 8;
                Vector2Int next = current + dirs[checkDir];
                if (Inside(next, width, height) && grid[next.x, next.y])
                {
                    current = next;
                    dir = checkDir;
                    contour.Add(ToLocal(current, mask.Rect, _pixelsPerUnit));
                    foundNext = true;
                    break;
                }
            }

            if (!foundNext)
                break;

            if (current == start && contour.Count > 3)
                finished = true;

        }
        while (!finished);

        return contour;
    }

    private static Vector2Int FindStartPixel(bool[,] grid, int width, int height)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (grid[x, y])
                {
                    // make sure it's a boundary pixel
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

    private List<Vector2> RemoveCollinearPoints(List<Vector2> points, float tolerance = 0.001f)
    {
        if (points == null || points.Count < 3)
            return points;

        List<Vector2> result = new List<Vector2>();
        int n = points.Count;

        for (int i = 0; i < n; i++)
        {
            Vector2 prev = points[(i - 1 + n) % n];
            Vector2 curr = points[i];
            Vector2 next = points[(i + 1) % n];

            // direction vectors
            Vector2 dir1 = (curr - prev).normalized;
            Vector2 dir2 = (next - curr).normalized;

            // check if they're nearly the same direction
            float dot = Vector2.Dot(dir1, dir2);

            // If directions are almost perfectly aligned to collinear
            if (Mathf.Abs(dot - 1f) < tolerance)
                continue;

            // Otherwise, keep the point
            result.Add(curr);
        }

        return result;
    }

}