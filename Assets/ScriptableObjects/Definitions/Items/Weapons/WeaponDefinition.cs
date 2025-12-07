using UnityEngine;

public abstract class WeaponDefinition : ItemDefinition
{
    public abstract bool IsRanged { get; }

    [Range(0f, 360f)] public float InitialVisualRotationDegrees;
    public override ItemType ItemType => ItemType.Weapon;
}
