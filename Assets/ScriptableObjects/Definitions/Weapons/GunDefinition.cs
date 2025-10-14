using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "GunDefinition", menuName = "Scriptable Objects/Collectibles/Weapons/GunDefinition")]
public class GunDefinition : ItemDefinition
{
    public RangedStatFloat FireStrength;
    public ProjectileDefinition ProjectileDefinition;
    public override IItemBehavior CreateItemBehavior()
    {
        return new GunBehavior(ProjectileDefinition.CreateProjectileBehavior(), this);
    }

    public override IEnumerable<RangedStat> GetRangedStats()
    {
        return ProjectileDefinition.GetRangedStats().Concat(new []{ FireStrength});
    }
}
