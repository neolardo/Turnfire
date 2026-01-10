using System;
using System.Collections;
using UnityEngine;

public abstract class WeaponBehavior : UnityDriven, IItemBehavior
{
    public bool IsInUse => _isAttacking;
    public bool IsAimingNormalized { get; protected set; }

    protected bool _isAttacking;

    public event Action ItemUsageFinished;

    protected WeaponBehavior(MonoBehaviour coroutineRunner) : base(coroutineRunner)
    {
    }

    protected void InvokeItemUsageFinished()
    {
        ItemUsageFinished?.Invoke();
    }

    public abstract void InitializePreview(ItemUsageContext context, ItemPreviewRendererManager rendererManager);

    public abstract void Use(ItemUsageContext context);

    public bool CanUseItem(ItemUsageContext context)
    {
        return true;
    }

    public abstract IEnumerator SimulateUsage(ItemBehaviorSimulationContext context, Action<ItemBehaviorSimulationResult> onDone);
}
