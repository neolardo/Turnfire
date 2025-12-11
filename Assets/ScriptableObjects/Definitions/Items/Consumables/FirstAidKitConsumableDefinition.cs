using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FirstAidKitConsumableDefinition", menuName = "Scriptable Objects/Items/Consumables/FirstAidKitConsumableDefinition")]
public class FirstAidKitConsumableDefinition : ConsumableDefinition
{
    public RangedStatInt HealAmount;
    public override IItemBehavior CreateItemBehavior()
    {
        return new FirstAidKitConsumableBehavior(this);
    }

    public override IEnumerable<RangedStat> GetRangedStats()
    {
        return new[] { HealAmount };
    }

}
