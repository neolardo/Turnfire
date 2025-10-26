using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelMask
{
    public bool[,] Mask { get; private set; }
    public RectInt Rect { get; private set; }
    public int Width => Rect.width;
    public int Height => Rect.height;

    public static IEnumerator CreateAsync(IEnumerable<Color> pixels, int width, int height, Action<PixelMask> onDone, int offsetX = 0, int offsetY = 0, int yieldInterval = 20000)
    {
        var rect = new RectInt(offsetX, offsetY, width, height);
        int ind = 0;
        var mask = new bool[width, height];
        foreach (var pixel in pixels)
        {
            int y = ind / width;
            int x = ind % width;
            mask[x, y] = pixel.a >= Constants.AlphaThreshold;
            ind++;
            if(ind % yieldInterval == 0)
            {
                yield return null;
            }
        }
        onDone?.Invoke(new PixelMask(rect, mask));
    }

    public static IEnumerator CreateAsync(IEnumerable<Vector2Int> pixels, Action<PixelMask> onDone, int yieldInterval = 20000)
    {
        int minX = int.MaxValue, maxX = int.MinValue;
        int minY = int.MaxValue, maxY = int.MinValue;
        int ind = 0;
        foreach (var p in pixels)
        {
            if (p.x < minX) minX = p.x;
            if (p.x > maxX) maxX = p.x;
            if (p.y < minY) minY = p.y;
            if (p.y > maxY) maxY = p.y;
            ind++;
            if(ind % yieldInterval == 0)
            {
                yield return null;
            }
        }

        int width = maxX - minX + 1;
        int height = maxY - minY + 1;
        var rect = new RectInt(minX, minY, width, height);
        var mask = new bool[width, height];
        ind = 0;
        foreach (var p in pixels)
        {
            int x = p.x - minX;
            int y = p.y - minY;
            mask[x, y] = true;
            ind++;
            if (ind % yieldInterval == 0)
            {
                yield return null;
            }
        }
        onDone?.Invoke(new PixelMask(rect, mask));
    }


    private PixelMask(RectInt rect, bool[,] mask) 
    {
        Rect = rect;
        Mask = mask;
    }

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
