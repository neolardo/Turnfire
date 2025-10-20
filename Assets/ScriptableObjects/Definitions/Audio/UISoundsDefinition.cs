using UnityEngine;

[CreateAssetMenu(fileName = "UISoundsDefinition", menuName = "Scriptable Objects/Audio/UISoundsDefinition")]
public class UISoundsDefinition : ScriptableObject
{
    public SFXDefiniton Hover;
    public SFXDefiniton Confirm;
    public SFXDefiniton Back;
    public SFXDefiniton Toggle;
    public SFXDefiniton InventoryOn;
    public SFXDefiniton InventoryOff;
}
