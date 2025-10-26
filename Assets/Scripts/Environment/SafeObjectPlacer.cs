using System.Collections.Generic;
using UnityEngine;

public static class SafeObjectPlacer
{
    private static float _searchRadius = 3;
    private static float _radiusStep = 0.2f;
    private static float _angleStep = 30f;
    private static int MaxRings => Mathf.CeilToInt(_searchRadius / _radiusStep);
    public static void SetSettings(float searchRadius, float radiusStep, float angleStep)
    {
        _searchRadius = searchRadius;
        _radiusStep = radiusStep;
        _angleStep = angleStep;
    }

    public static bool TryFindSafePosition(Vector2 center, Vector2 direction, LayerMask collisionMask, float requiredRadius, out Vector2 safePosition)
    {
        float baseAngle = Mathf.Atan2(direction.y, direction.x);

        for (int ring = 0; ring < MaxRings; ring++)
        {
            float ringRadius = _radiusStep * (ring + 1);

            // Generate alternating angles around base direction
            List<float> candidateAngles = GenerateAlternatingAngles(baseAngle, _angleStep, 180f / _angleStep);

            foreach (float angle in candidateAngles)
            {
                Vector2 candidate = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * ringRadius;
               
                if (IsPositionSafe(candidate, requiredRadius, collisionMask))
                {
                    safePosition = candidate;
                    return true;
                }
            }
        }
        safePosition = new Vector2(-1, -1);
        return false;
    }

    //TODO: optimize
    private static bool IsPositionSafe(Vector2 pos, float radius, LayerMask mask)
    {
        if (Physics2D.OverlapPointAll(pos, LayerMaskHelper.GetLayerMask(Constants.GroundLayer)).Length > 0)
            return false;

        if (Physics2D.OverlapCircleAll(pos, radius, LayerMaskHelper.GetLayerMask(Constants.GroundLayer)).Length > 0)
            return false;

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
