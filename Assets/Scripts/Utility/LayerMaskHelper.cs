using System.Collections;
using UnityEngine;

public static class LayerMaskHelper
{
    public static LayerMask GetLayerMask(int layer)
    {
        return 1 << layer;
    }

    public static LayerMask GetCombinedLayerMask(params int[] layers)
    {
        int mask = 0;
        foreach (int layer in layers)
        {
            mask |= GetLayerMask(layer);
        }
        return mask;
    }
}
