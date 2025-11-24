using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBehavior : UnityDriven, IItemBehavior
{
    public bool IsInUse => _isFiring;

    protected bool _isFiring;

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

    public abstract Vector2 SimulateWeaponBehaviorAndCalculateDestination(Vector2 start, Vector2 aimVector, DestructibleTerrainManager terrain, Character owner, IEnumerable<Character> others, bool asd);
}
