using UnityEngine;
using UnityEngine.UI;

public class TransparentRaycastRawImage : RawImage, ICanvasRaycastFilter
{
    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        return true;
    }
}
