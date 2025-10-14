using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BulletDefinition", menuName = "Scriptable Objects/Projectiles/BulletDefinition")]
public class BulletProjectileDefinition : ProjectileDefinition
{
    public override IProjectileBehavior CreateProjectileBehavior()
    {
        return new BulletProjectileBehavior(this);
    }

    public override IEnumerable<RangedStat> GetRangedStats()
    {
        return new [] { Damage};
    }
}
