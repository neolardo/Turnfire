using UnityEngine;
using UnityEngine.UIElements;

public class TerrainTexture
{
    public Texture2D Texture { get; }
    public Vector2 CenteredPivotOffset { get; }
    public int PixelsPerUnit { get; }

    private int _width => Texture.width;
    private int _height => Texture.height;

    public TerrainTexture(Texture2D texture, Vector2 pivotOffset, int pixelsPerUnit)
    {
        Texture = texture;
        CenteredPivotOffset = pivotOffset;
        PixelsPerUnit = pixelsPerUnit;
    }

    public bool IsSolidPixel(int x, int y) => !IsPixelOutOfBounds(x, y) && Texture.GetPixel(x, y).a >= Constants.AlphaThreshold;
    public bool IsSolidPixel(Vector2Int p) => IsSolidPixel(p.x, p.y);

    public bool IsPixelOutOfBounds(int x, int y) => x < 0 || y < 0 || x >= _width || y >= _height;
    public bool IsPixelOutOfBounds(Vector2Int p) => IsPixelOutOfBounds(p.x, p.y);

    public void SetPixelSafe(int x, int y, Color color)
    {
        if (!IsPixelOutOfBounds(x, y)) Texture.SetPixel(x, y, color);
    }

    public Vector2Int LocalPointToPixel(Vector2 local)
    {
        return new Vector2Int(
            Mathf.RoundToInt(local.x * PixelsPerUnit + _width / 2f),
            Mathf.RoundToInt(local.y * PixelsPerUnit + _height / 2f)
        );
    }

    public Vector2 PixelToLocalPoint(Vector2Int pixel)
    {
        return new Vector2(
            (pixel.x - _width / 2f) / PixelsPerUnit,
            (pixel.y - _height / 2f) / PixelsPerUnit
        );
    }

    public Vector2 WorldToLocal(Vector2 world)
    {
        return world - CenteredPivotOffset;
    }
    public Vector2 LocalToWorld(Vector2 local)
    {
        return local + CenteredPivotOffset;
    }

    public void ClearCircle(Vector2 localCenter, float radius)
    {
        Vector2Int centerPixel = LocalPointToPixel(localCenter);
        int r = Mathf.RoundToInt(radius * PixelsPerUnit);

        for (int y = -r; y <= r; y++)
            for (int x = -r; x <= r; x++)
                if (x * x + y * y <= r * r)
                    SetPixelSafe(centerPixel.x + x, centerPixel.y + y, Color.clear);

        Texture.Apply();
    }
}
