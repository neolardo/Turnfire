using UnityEngine;

public static class MathHelpers
{
    public static float Sigmoid(float x, float decayMidpoint, float steepness)
    {
        return 1f / (1 + Mathf.Exp(steepness * (x - decayMidpoint))); 
    }
}
