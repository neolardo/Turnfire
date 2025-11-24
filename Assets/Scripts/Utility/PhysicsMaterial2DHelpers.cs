using UnityEngine;

public static class PhysicsMaterial2DHelpers
{
    private const float ExtraTangentDamping = 0.75f;
    private const float NormalMagnitudeBounceThreshold = 1.5f;

    public static Vector2 ApplyMaterialBounce(Vector2 velocity, Vector2 normal, PhysicsMaterial2D mat)
    {
        normal = normal.normalized;

        // Decompose velocity into normal/tangent components
        float vN = Vector2.Dot(velocity, normal);
        Vector2 vNormal = vN * normal;
        Vector2 vTangent = velocity - vNormal;


        if (vN < 0f)
        {
            vNormal = -vNormal * mat.bounciness;
        }

        if (Mathf.Abs(vN) < NormalMagnitudeBounceThreshold)
        {
            vNormal = Vector2.zero;
        }

        float frictionFactor = Mathf.Clamp01(mat.friction * 0.5f);
        vTangent *= (1f - frictionFactor) * ExtraTangentDamping;

        return (vNormal + vTangent) ;
    }
}
