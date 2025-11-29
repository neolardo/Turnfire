public class FirstAidKitConsumableBehavior : ConsumableBehavior
{
    FirstAidKitConsumableDefinition _definition;
    public FirstAidKitConsumableBehavior(FirstAidKitConsumableDefinition definition)
    {
        _definition = definition;
    }
    
    public override void Use(ItemUsageContext context)
    {
        IsInUse = true;
        context.Owner.Heal(_definition.HealAmount.CalculateValue());
        //TODO: wait for animation
        InvokeItemUsageFinished();
    }
}
