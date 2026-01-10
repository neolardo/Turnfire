using UnityEngine;

public abstract class WeaponDefinition : ItemDefinition, IDamageSourceDefinition
{
    [SerializeField] private SFXDefiniton _hitSFX;
    public SFXDefiniton HitSFX => _hitSFX;
    public abstract bool IsRanged { get; }

    [Range(0f, 360f)] public float InitialVisualRotationDegrees;
    public override ItemType ItemType => ItemType.Weapon;
}
