using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LaserGunDefinition", menuName = "Scriptable Objects/Items/Weapons/LaserGunDefinition")]
public class LaserGunWeaponDefinition : WeaponDefinition
{
    public RangedStatInt Damage;
    public RangedStatInt MaximumBounceCount;
    public RangedStatFloat MaximumDistance;

    public override IItemBehavior CreateItemBehavior()
    {
        return new LaserGunWeaponBehavior(this);
    }

    public override IEnumerable<RangedStat> GetRangedStats()
    {
        return new[] {Damage};
    }
}
