using System;
using System.Collections;
using UnityEngine;

public abstract class ConsumableBehavior : UnityDriven,IItemBehavior
{
    public bool IsInUse { get;protected set; }

    public event Action ItemUsageFinished;

    private ConsumableDefinition _definition;

    protected ConsumableBehavior(ConsumableDefinition definition) : base(CoroutineRunner.Instance)
    {
        _definition = definition;
    }

    public virtual bool CanUseItem(ItemUsageContext context)
    {
        return true;
    }

    public virtual void InitializePreview(ItemUsageContext context, ItemPreviewRendererManager rendererManager)
    { }

    public abstract void Use(ItemUsageContext context);

    protected void InvokeItemUsageFinished()
    {
        StartCoroutine(WaitForItemUsageDelayThenInvokeFinished());
    }

    private IEnumerator WaitForItemUsageDelayThenInvokeFinished()
    {
        yield return new WaitForSeconds(_definition.ItemUsagePostDelay);
        IsInUse = false;
        ItemUsageFinished?.Invoke();
    }

    public abstract IEnumerator SimulateUsage(ItemBehaviorSimulationContext context, Action<ItemBehaviorSimulationResult> onDone);
}
