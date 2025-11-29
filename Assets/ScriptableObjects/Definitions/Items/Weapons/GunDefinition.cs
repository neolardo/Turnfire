using UnityEngine;

[CreateAssetMenu(fileName = "GunDefinition", menuName = "Scriptable Objects/Collectibles/Weapons/GunDefinition")]
public class GunDefinition : ProjectileLauncherWeaponDefinition
{
    public override IItemBehavior CreateItemBehavior()
    {
        return new GunWeaponBehavior(ProjectileDefinition.CreateProjectileBehavior(), this);
    }
}