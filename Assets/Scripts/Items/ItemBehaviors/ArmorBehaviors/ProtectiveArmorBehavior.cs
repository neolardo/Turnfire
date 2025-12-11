using System;
using System.Collections;

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
        OnItemUsageFinished();
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

    public override IEnumerator SimulateUsage(ItemBehaviorSimulationContext context, Action<ItemBehaviorSimulationResult> onDone)
    {
        if (context.Owner.ArmorManager.CanEquip(_definition))
        {
            onDone?.Invoke(ItemBehaviorSimulationResult.ArmorBoost(_definition.MaxDurability.AvarageValue));
        }
        else
        {
            onDone?.Invoke(ItemBehaviorSimulationResult.None);
        }
        yield return null;
    }

}

