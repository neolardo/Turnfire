using UnityEngine;

[CreateAssetMenu(fileName = "ItemPreviewRendererSettingsDefinition", menuName = "Scriptable Objects/ItemPreviewRendererSettingsDefinition")]
public class ItemPreviewRendererSettingsDefinition : ScriptableObject
{
    public float AimCircleOuterRadiusPercent = 0.06f;
    public float AimCircleInnerRadiusPercent = 0.018f;
    public float DefaultAimCircleOffsetPercentX = 0.10f;
    public float DefaultAimCircleOffsetPercentY = 0.85f;
}
