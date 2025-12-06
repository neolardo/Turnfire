using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBehavior : UnityDriven, IItemBehavior
{
    public bool IsInUse => _isAttacking;

    protected bool _isAttacking;

    protected WeaponBehavior(MonoBehaviour coroutineRunner) : base(coroutineRunner)
    {
    }

    public event Action ItemUsageFinished;

    protected void InvokeItemUsageFinished()
    {
        ItemUsageFinished?.Invoke();
    }

    public abstract void InitializePreview(ItemUsageContext context, ItemPreviewRendererManager rendererManager);

    public abstract void Use(ItemUsageContext context);

    public abstract WeaponBehaviorSimulationResult SimulateWeaponBehavior(Vector2 start, Vector2 aimVector, DestructibleTerrainManager terrain, Character owner, IEnumerable<Character> others);

    public bool CanUseItem(ItemUsageContext context)
    {
        return true;
    }
}
