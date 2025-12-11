using UnityEngine;

[CreateAssetMenu(fileName = "ProtectiveArmorDefinition", menuName = "Scriptable Objects/Items/Armors/ProtectiveArmorDefinition")]
public class ProtectiveArmorDefinition : ArmorDefinition
{
    public override bool IsProtective => true;
    public override IItemBehavior CreateItemBehavior()
    {
        return new ProtectiveArmorBehavior(this);
    }

}
