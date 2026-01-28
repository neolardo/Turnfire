using UnityEngine;

public class TerrainPhysics
{
    private readonly TerrainTexture _texture;

    private readonly Vector2Int[] _normalNeighbors = {
        new(-1,  0), new( 1,  0), new( 0, -1), new( 0,  1),
        new(-1, -1), new(-1,  1), new( 1, -1), new( 1,  1)
    };

    private readonly Vector2Int[] _edgeNeighbors = {
        new(1,0), new(-1,0), new(0,1), new(0,-1)
    };

    private const float OverlapCheckAngleStep = 45f;
    private const int MaxSearchRadiusForNormalCalculation = 4;
    private const int CornerCheckDepth = 3;
    private const int DefaultCornerMinHalfWidth = 8;

    public TerrainPhysics(TerrainTexture texture)
    {
        _texture = texture;
    }

    #region Overlap Check

    public bool OverlapPoint(Vector2 localPos) => IsSolidPixel(localPos);
    public bool OverlapCircle(Vector2 localCenter, float radius)
    {
        float angle = 0;
        while (angle < 360)
        {
            var point = localCenter + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            if (IsSolidPixel(point))
            {
                return true;
            }
            angle += OverlapCheckAngleStep;
        }
        return false;
    }

    private bool IsSolidPixel(Vector2 localPos) => _texture.IsSolidPixel(_texture.LocalPointToPixel(localPos));

    #endregion

    #region Normal Calculation

    public Vector2 GetNearestNormal(Vector2 localPos)
    {
        Vector2Int origin = _texture.LocalPointToPixel(localPos);
        if (!TryFindNearestSolidEdgePixel(origin, MaxSearchRadiusForNormalCalculation, out Vector2Int nearest))
        {
            return Vector2.up;
        }
        return ComputeNormal(nearest);
    }

    private bool TryFindNearestSolidEdgePixel(Vector2Int origin, int radius, out Vector2Int edgePixel)
    {
        edgePixel = origin;
        for (int r = 0; r <= radius; r++)
        {
            for (int dx = -r; dx <= r; dx++)
            {
                for (int dy = -r; dy <= r; dy++)
                {
                    if (Mathf.Abs(dx) != r && Mathf.Abs(dy) != r)
                    {
                        continue;// check once per ring
                    }
                    Vector2Int p = origin + new Vector2Int(dx, dy);
                    if (_texture.IsPixelOutOfBounds(p) || !_texture.IsSolidPixel(p))
                    {
                        continue;
                    }
                    if (IsEdgePixel(p))
                    {
                        edgePixel = p; 
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private bool IsEdgePixel(Vector2Int p)
    {
        foreach (var n in _edgeNeighbors)
        {
            Vector2Int np = p + n;
            if (_texture.IsPixelOutOfBounds(np) || !_texture.IsSolidPixel(np))
            {
                return true;
            }
        }
        return false;
    }

    private Vector2 ComputeNormal(Vector2Int point)
    {
        Vector2 normal = Vector2.zero;
        foreach (var n in _normalNeighbors)
        {
            Vector2Int np = point + n;
            if (_texture.IsPixelOutOfBounds(np) || !_texture.IsSolidPixel(np))
            {
                normal += (Vector2)n;
            }
        }
        if (Mathf.Approximately(normal.sqrMagnitude, 0f))
        {
            return Vector2.up;
        }
        return normal.normalized;
    }

    #endregion

    #region Standing Point Calculation

    public bool TryFindNearestStandingPoint(Vector2Int pixel, int searchRadius, int standingPointId, out StandingPoint standingPoint)
    {
        standingPoint = default;
        int nonCornerHalfWidth = (int)(StandingPoint.NonCornerPointNeighbourHalfWidth * _texture.PixelsPerUnit);

        for (int r = 0; r <= searchRadius; r++)
        {
            for (int dx = -r; dx <= r; dx++)
            {
                for (int dy = -r; dy <= r; dy++)
                {
                    if (Mathf.Abs(dx) != r && Mathf.Abs(dy) != r)
                        continue; // check once per ring

                    Vector2Int p = pixel + new Vector2Int(dx, dy);
                    if (_texture.IsPixelOutOfBounds(p) || !_texture.IsSolidPixel(p) || !IsEdgePixel(p) || IsHorizontalCornerPixel(p))
                        continue;

                    if (StandingPoint.IsStandingNormal(ComputeNormal(p)))
                    {
                        Vector2 localPos = _texture.PixelToLocalPoint(p);
                        Vector2 worldPos = _texture.LocalToWorld(localPos);
                        bool isCorner = IsHorizontalCornerPixel(p, nonCornerHalfWidth);
                        standingPoint = new StandingPoint(standingPointId, worldPos, p, isCorner);
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public bool IsCorner(Vector2 localPos)
    {
        Vector2Int p = _texture.LocalPointToPixel(localPos);
        int minHalfWidth = (int)(StandingPoint.NonCornerPointNeighbourHalfWidth * _texture.PixelsPerUnit);
        return IsHorizontalCornerPixel(p, minHalfWidth);
    }

    private bool IsHorizontalCornerPixel(Vector2Int p, int minHalfWidth = DefaultCornerMinHalfWidth)
    {
        int y = p.y - CornerCheckDepth;

        for (int nx = 1; nx <= minHalfWidth; nx++)
        {
            if (!_texture.IsSolidPixel(p.x + nx, y) || !_texture.IsSolidPixel(p.x - nx, y))
            {
                return true; 
            }
        }

        return false;
    }

    #endregion
}
