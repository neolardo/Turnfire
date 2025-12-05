public class BootsArmorBehavior : ArmorBehavior
{
    public BootsArmorDefinition _definition;
    public BootsArmorBehavior(BootsArmorDefinition definition) : base(definition)
    {
        _definition = definition;
    }
    public override void Use(ItemUsageContext context)
    {
        base.Use(context);
        _owner.ArmorManager.TryEquipArmor(_definition, this);
        _owner.AddJumpBoost(_definition.AdditionalJumpRange.CalculateValue());
        _owner.Jumped += OnOwnerJumped;
        OnItemUsageFinished();
    }

    private void OnOwnerJumped()
    {
        DecreaseDurability();
    }

    protected override void OnArmorWornOut()
    {
        base.OnArmorWornOut();
        _owner.RemoveJumpBoost();
        _owner.Jumped -= OnOwnerJumped;
    }
}
