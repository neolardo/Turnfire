using System;
using System.Collections;
using UnityEngine;

public abstract class ArmorBehavior : UnityDriven,IItemBehavior
{
    protected int _durability;
    protected Character _owner;
    public bool IsInUse {get; protected set;}

    public event Action ItemUsageFinished;
    public event Action<ArmorDefinition> ArmorWornOut;

    private ArmorDefinition _definition;

    protected ArmorBehavior(ArmorDefinition definition) : base(CoroutineRunner.Instance)
    {
        _definition = definition;
        _durability = _definition.MaxDurability.CalculateValue();
    }

    public void InitializePreview(ItemUsageContext context, ItemPreviewRendererManager rendererManager){}

    public virtual bool CanUseItem(ItemUsageContext context)
    {
        return context.Owner.CanEquipArmor(_definition);
    }

    public virtual void Use(ItemUsageContext context)
    {
        IsInUse = true;
        _owner = context.Owner;
    }
    protected virtual void OnArmorWornOut()
    {
        ArmorWornOut?.Invoke(_definition);
    }

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

    protected void DecreaseDurability()
    {
        _durability--;
        if(_durability == 0) 
        {
            OnArmorWornOut();
        }
    }

    public abstract IEnumerator SimulateUsage(ItemBehaviorSimulationContext context, Action<ItemBehaviorSimulationResult> onDone);

}
