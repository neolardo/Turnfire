public abstract class WeaponDefinition : ItemDefinition
{
    public abstract bool IsRanged { get; }
    public override ItemType ItemType => ItemType.Weapon;
}
