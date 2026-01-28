using System.Collections.Generic;
using UnityEngine;

public static class SafeObjectPlacer
{
    private static float _searchRadius = 3;
    private static float _radiusStep = 0.2f;
    private static float _angleStep = 30f;

    private const int OverlapColliderCountMax = 10;
    private const float Epsilon = 0.01f;

    private static readonly Collider2D[] _overlapColliders = new Collider2D[OverlapColliderCountMax];

    private static int MaxRings => Mathf.CeilToInt(_searchRadius / _radiusStep);

    private static TerrainManager _destructibleTerrain;

    public static void SetSettings(float searchRadius, float radiusStep, float angleStep)
    {
        _searchRadius = searchRadius;
        _radiusStep = radiusStep;
        _angleStep = angleStep;
    }

    public static void SetDestructibleTerrain(TerrainManager destructibleTerrain)
    {
        _destructibleTerrain = destructibleTerrain;
    }

    public static bool TryFindSafePosition(Vector2 center, Vector2 direction, LayerMask overlapMask, float requiredRadius, out Vector2 safePosition)
    {
        bool checkGround = overlapMask.HasLayer(Constants.GroundLayer);
        if(checkGround)
        {
            overlapMask = overlapMask.RemoveLayer(Constants.GroundLayer);
        }

        var filter = new ContactFilter2D();
        filter.SetLayerMask(overlapMask);
        Physics2D.OverlapCircle(center, _searchRadius, filter, _overlapColliders);

        float baseAngle = Mathf.Atan2(direction.y, direction.x);
        var candidateAngles = GenerateAlternatingAngles(baseAngle, _angleStep, 180f / _angleStep);

        for (int ring = 0; ring < MaxRings; ring++)
        {
            float ringRadius = _radiusStep * (ring + 1);

            foreach (float angle in candidateAngles)
            {
                Vector2 candidate = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * ringRadius;
               
                if (IsPositionSafe(candidate, requiredRadius, checkGround))
                {
                    safePosition = candidate;
                    return true;
                }
            }
        }
        safePosition = new Vector2(-1, -1);
        return false;
    }

    private static bool IsPositionSafe(Vector2 pos, float radius, bool checkGround)
    {
        foreach (var col in _overlapColliders)
        {
            if (!col) continue;

            Vector2 closest = col.bounds.ClosestPoint(pos);
            float distSqr = (pos - closest).sqrMagnitude;
            float required = radius + Epsilon;

            if (distSqr < required * required)
            {
                return false;
            }
        }

        if (checkGround)
        {
            if(_destructibleTerrain.OverlapPoint(pos))
            {
                return false;
            }
            if (_destructibleTerrain.OverlapCircle(pos, radius))
            {
                return false;
            }
        }

        return true;
    }

    private static List<float> GenerateAlternatingAngles(float baseAngle, float stepDeg, float maxSteps)
    {
        var angles = new List<float>();
        float stepRad = stepDeg * Mathf.Deg2Rad;

        for (int i = 0; i < maxSteps; i++)
        {
            int offset = (i + 1) / 2;
            bool isPositive = (i % 2 == 1);
            float angle = baseAngle + (isPositive ? offset * stepRad : -offset * stepRad);
            angles.Add(angle);
        }

        angles.Insert(0, baseAngle);
        return angles;
    }
}
