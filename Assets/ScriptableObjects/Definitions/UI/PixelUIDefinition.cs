using UnityEngine;

[CreateAssetMenu(fileName = "PixelUIDefinition", menuName = "Scriptable Objects/PixelUIDefinition")]
public class PixelUIDefinition : ScriptableObject
{
    public int PixelsPerUnit;
    public int TargetScreenHeightInPixels;
    public int HoverPixelOffset;

    public Vector2 CalculateHoverOffset(float canvasHeight) => new Vector2(0, (canvasHeight / TargetScreenHeightInPixels) * HoverPixelOffset);
}
