using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MeleeWeaponDefinition", menuName = "Scriptable Objects/Items/Weapons/MeleeWeaponDefinition")]
public class MeleeWeaponDefinition : WeaponDefinition
{
    public override bool IsRanged => false;

    public RangedStatInt Damage;
    public RangedStatFloat AttackRange;
    public RangedStatFloat AttackSectorAngleDegrees;
    public override IItemBehavior CreateItemBehavior()
    {
        return new MeleeWeaponBehavior(this);
    }

    public override IEnumerable<RangedStat> GetRangedStats()
    {
        return new RangedStat[] { AttackRange, Damage };
    }
}
