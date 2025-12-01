public class ProtectiveArmorBehavior : ArmorBehavior
{
    private ProtectiveArmorDefinition _definition;
    public ProtectiveArmorBehavior(ProtectiveArmorDefinition definition) : base(definition)
    {
        _definition = definition;
    }

    public override void Use(ItemUsageContext context)
    {
        base.Use(context);
        _owner.ArmorManager.BlockedWithArmor += OnBlockedWithArmor;
        _owner.ArmorManager.TryEquipArmor(_definition, this);
        InvokeItemUsageFinished();
    }

    private void OnBlockedWithArmor(ArmorDefinition armorDefinition)
    {
        if (armorDefinition == _definition)
        {
            DecreaseDurability();
        }
    }

    protected override void OnArmorWornOut()
    {
        base.OnArmorWornOut();
        _owner.ArmorManager.BlockedWithArmor -= OnBlockedWithArmor;
    }

}

