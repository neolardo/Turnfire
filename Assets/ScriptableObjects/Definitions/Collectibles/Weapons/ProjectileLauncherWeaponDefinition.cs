using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileLauncherWeaponDefinition", menuName = "Scriptable Objects/Collectibles/Weapons/ProjectileLauncherWeaponDefinition")]
public class ProjectileLauncherWeaponDefinition : WeaponDefinition
{
    public RangedStatFloat FireStrength;
    public ProjectileDefinition ProjectileDefinition;
    public bool UseGravityForPreview = true;

    public override IItemBehavior CreateItemBehavior()
    {
        return new ProjectileLauncherWeaponBehavior(ProjectileDefinition.CreateProjectileBehavior(), this);
    }

    public override IEnumerable<RangedStat> GetRangedStats()
    {
        return ProjectileDefinition.GetRangedStats().Concat(new[] { FireStrength });
    }
}
