using UnityEngine;

public readonly struct StandingPoint 
{
    public readonly int Id;
    public readonly Vector2 WorldPos;
    public readonly Vector2Int PixelCoordinates;
    public readonly bool IsCornerPoint;
    public bool IsValid => Id >= 0;

    private const float StandingNormalYMin = 0.79f;
    public const float NonCornerPointNeighbourHalfWidth = .25f;

    public static readonly StandingPoint InvalidPoint = new StandingPoint(-1, Vector2.zero, Vector2Int.zero, false);

    public StandingPoint(int id, Vector2 worldPos, Vector2Int pixelCoordinate, bool isCornerPoint)
    {
        Id = id;
        WorldPos = worldPos;
        PixelCoordinates = pixelCoordinate;
        IsCornerPoint = isCornerPoint;
    }

    public static bool IsStandingNormal(Vector2 normal)
    {
        return normal.y >= StandingNormalYMin;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        if (obj is null) return false;
        if (obj.GetType() != GetType()) return false;

        var other = (StandingPoint)obj;
        return other == this;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + Id.GetHashCode();
            return hash;
        }
    }

    public static bool operator== (StandingPoint a, StandingPoint b)
    {
        return a.Id == b.Id;
    }

    public static bool operator!= (StandingPoint a, StandingPoint b)
    {
        return a.Id != b.Id;
    }
}
