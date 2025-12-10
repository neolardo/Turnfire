using UnityEngine;
using UnityEngine.UI;

public class TransparentRaycastImage : Image, ICanvasRaycastFilter
{
    public override bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        return true;
    }
}
