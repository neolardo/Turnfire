using UnityEngine;

public readonly struct StandingPoint 
{
    public readonly int Id;
    public readonly Vector2 WorldPos;
    public readonly Vector2Int PixelCoordinate;
    public readonly bool IsCornerPoint;
    public bool IsValid => Id >= 0;

    private const float StandingNormalYMin = 0.85f;
    public const float NonCornerPointNeighbourHalfWidth = .25f;

    public static readonly StandingPoint InvalidPoint = new StandingPoint(-1, Vector2.zero, Vector2Int.zero, false);

    public StandingPoint(int id, Vector2 worldPos, Vector2Int pixelCoordinate, bool isCornerPoint)
    {
        Id = id;
        WorldPos = worldPos;
        PixelCoordinate = pixelCoordinate;
        IsCornerPoint = isCornerPoint;
    }

    public static bool IsStandingNormal(Vector2 normal)
    {
        return normal.y >= StandingNormalYMin;
    }


}
