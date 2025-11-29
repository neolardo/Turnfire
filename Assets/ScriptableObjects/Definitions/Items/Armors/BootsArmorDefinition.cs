using UnityEngine;
[CreateAssetMenu(fileName = "BootsArmorDefinition", menuName = "Scriptable Objects/Items/Armors/BootsArmorDefinition")]
public class BootsArmorDefinition : ArmorDefinition
{
    public RangedStatFloat AdditionalJumpRange;
    public override bool IsProtective => false;
    public override IItemBehavior CreateItemBehavior()
    {
        return new BootsArmorBehavior(this);
    }

}
