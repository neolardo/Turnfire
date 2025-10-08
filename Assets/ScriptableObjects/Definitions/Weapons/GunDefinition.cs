using UnityEngine;

[CreateAssetMenu(fileName = "GunDefinition", menuName = "Scriptable Objects/Collectibles/Weapons/GunDefinition")]
public class GunDefinition : ItemDefinition
{
    public float FireStrength;

    public ProjectileDefinition ProjectileDefinition;
    public override IItemBehavior CreateItemBehavior()
    {
        return new GunBehavior(ProjectileDefinition.CreateProjectileBehavior(), this);
    }
}
