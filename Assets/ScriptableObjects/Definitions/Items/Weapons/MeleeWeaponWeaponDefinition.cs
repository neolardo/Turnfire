using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MeleeWeaponDefinition", menuName = "Scriptable Objects/Items/Weapons/MeleeWeaponDefinition")]
public class MeleeWeaponWeaponDefinition : WeaponDefinition
{
    public RangedStatInt Damage;
    public RangedStatFloat AttackRange;
    public override IItemBehavior CreateItemBehavior()
    {
        return new MeleeWeaponWeaponBehavior(this);
    }

    public override IEnumerable<RangedStat> GetRangedStats()
    {
        return new RangedStat[] { AttackRange, Damage };
    }
}
