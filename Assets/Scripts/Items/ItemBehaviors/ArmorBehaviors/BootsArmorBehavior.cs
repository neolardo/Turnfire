public class BootsArmorBehavior : ArmorBehavior
{
    public BootsArmorDefinition _definition;
    public BootsArmorBehavior(BootsArmorDefinition definition)
    {
        _definition = definition;
    }
    public override void Use(ItemUsageContext context)
    {
        base.Use(context);
        _owner.AddJumpBoost(_definition.AdditionalJumpRange.CalculateValue());
        _owner.Jumped += OnOwnerJumped;
        //TODO: wait for animation
        InvokeItemUsageFinished();
    }

    private void OnOwnerJumped()
    {
        DecreaseDurability();
    }

    protected override void OnOwnerDied()
    {
        _owner.Jumped -= OnOwnerJumped;
    }

    protected override void OnArmorWornOut()
    {
        _owner.RemoveJumpBoost();
        _owner.Jumped -= OnOwnerJumped;
    }
}
