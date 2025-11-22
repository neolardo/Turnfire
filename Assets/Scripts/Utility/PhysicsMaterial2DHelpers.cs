using UnityEngine;

public static class PhysicsMaterial2DHelpers
{
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
        vTangent *= (1f - Mathf.Clamp01(mat.friction));

        return vNormal + vTangent;
    }
}
