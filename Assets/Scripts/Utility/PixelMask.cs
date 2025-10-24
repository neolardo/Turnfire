using System.Collections.Generic;
using UnityEngine;

public class PixelMask
{
    public bool[,] Mask { get; private set; }
    public RectInt Rect { get; private set; }
    public int Width => Rect.width;
    public int Height => Rect.height;

    public PixelMask(int width, int height, int offsetX = 0, int offsetY = 0)
    {
        Rect = new RectInt(offsetX, offsetY, width, height);
        Mask = new bool[Width, Height];
    }

    public PixelMask(IEnumerable<Color> pixels, int width, int height, int offsetX = 0, int offsetY = 0)
    {
        Rect = new RectInt(offsetX, offsetY, width, height);
        int ind = 0;
        Mask = new bool[Width, Height];
        foreach (var pixel in pixels)
        {
            int y = ind / width;
            int x = ind % width;
            Mask[x, y] = pixel.a >= Constants.AlphaThreshold;
            ind++;
        }
    }

    public PixelMask(IEnumerable<Vector2Int> pixels)
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
        Rect = new RectInt(minX, minY, width, height);
        Mask = new bool[width, height];
        foreach (var p in pixels)
        {
            int x = p.x - minX;
            int y = p.y - minY;
            Mask[x, y] = true;
        }
    }

    public bool this[int x, int y]
    {
        get
        {
            return Mask[x, y];
        }
        set
        {
            Mask[x, y] = value;
        }
    }
}
