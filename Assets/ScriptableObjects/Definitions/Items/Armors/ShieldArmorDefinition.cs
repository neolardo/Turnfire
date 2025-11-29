using UnityEngine;

[CreateAssetMenu(fileName = "ShieldArmorDefinition", menuName = "Scriptable Objects/Items/Armors/ShieldArmorDefinition")]
public class ShieldArmorDefinition : ArmorDefinition
{
    public override bool IsProtective => true;
    public override IItemBehavior CreateItemBehavior()
    {
        return new ShieldArmorBehavior(this);
    }

}
