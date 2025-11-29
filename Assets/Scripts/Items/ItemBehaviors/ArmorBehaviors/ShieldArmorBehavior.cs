public class ShieldArmorBehavior : ArmorBehavior
{
    private ShieldArmorDefinition _definition;
    public ShieldArmorBehavior(ShieldArmorDefinition definition)
    {
        _definition = definition;
    }
    public override void Use(ItemUsageContext context)
    {
        base.Use(context);
        _owner.TryEquipArmor() //TODO
        _owner.BlockedWithArmor += OnOwnerBlockedWithArmor;
        //TODO: wait for animation
        InvokeItemUsageFinished();
    }


    private void OnOwnerBlockedWithArmor(Item armor)
    {
        if(armor.Definition == this._definition)
        {
            DecreaseDurability();
        }
    }

    protected override void OnArmorWornOut()
    {
        _owner.ToggleProtection(false);
        _owner.Dodged -= OnOwnerDodged;
    }

    protected override void OnOwnerDied()
    {
        _owner.Dodged -= OnOwnerDodged;
    }
}
