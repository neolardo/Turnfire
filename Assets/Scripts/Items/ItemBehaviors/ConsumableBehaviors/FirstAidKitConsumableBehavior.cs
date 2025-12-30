using System;
using System.Collections;

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
        BotEvaluationStatistics.GetData(context.Owner.Team).ConsumablesUsedCount++;
        UnityEngine.Debug.Log($"First aid kit used");
        context.Owner.Heal(_definition.HealAmount.CalculateValue());
        InvokeItemUsageFinished();
    }

    public override IEnumerator SimulateUsage(ItemBehaviorSimulationContext context, Action<ItemBehaviorSimulationResult> onDone)
    {
        onDone?.Invoke(ItemBehaviorSimulationResult.Healing(_definition.HealAmount.AvarageValue));
        yield return null;
    }
}
