using System;
using System.Collections;

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
        _owner.TryEquipArmor(_definition, this);
        _owner.ApplyJumpBoost(_definition.AdditionalJumpRange.CalculateValue());
        _owner.Jumped += OnOwnerJumped;
        InvokeItemUsageFinished();
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

    public override IEnumerator SimulateUsage(ItemBehaviorSimulationContext context, Action<ItemBehaviorSimulationResult> onDone)
    {
        if(context.Owner.CanEquipArmor(_definition))
        {
            onDone?.Invoke(ItemBehaviorSimulationResult.MobilityBoost(_definition.AdditionalJumpRange.AvarageValue));
        }
        else 
        {
            onDone?.Invoke(ItemBehaviorSimulationResult.None);
        }
        yield return null;
    }

}
