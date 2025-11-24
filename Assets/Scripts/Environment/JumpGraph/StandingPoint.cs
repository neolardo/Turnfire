using UnityEngine;

public readonly struct StandingPoint 
{
    public readonly int Id;
    public readonly Vector2 WorldPos;
    public readonly Vector2Int PixelCoordinate;

    private const float StandingNormalYMin = 0.85f;

    public StandingPoint(int id, Vector2 worldPos, Vector2Int pixelCoordinate)
    {
        Id = id;
        WorldPos = worldPos;
        PixelCoordinate = pixelCoordinate;
    }

    public static bool IsStandingNormal(Vector2 normal)
    {
        return normal.y >= StandingNormalYMin;
    }
}
