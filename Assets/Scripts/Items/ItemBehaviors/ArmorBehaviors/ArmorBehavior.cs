using System;
using System.Collections;

public abstract class ArmorBehavior : IItemBehavior
{
    protected int _durability;
    protected Character _owner;
    public bool IsInUse {get; protected set;}

    public event Action ItemUsageFinished;
    public event Action<ArmorDefinition> ArmorWornOut;

    private ArmorDefinition _definition;

    protected ArmorBehavior(ArmorDefinition definition)
    {
        _definition = definition;
        _durability = _definition.MaxDurability.CalculateValue();
    }

    public void InitializePreview(ItemUsageContext context, ItemPreviewRendererManager rendererManager){}

    public virtual bool CanUseItem(ItemUsageContext context)
    {
        return context.Owner.ArmorManager.CanEquip(_definition);
    }

    public virtual void Use(ItemUsageContext context)
    {
        IsInUse = true;
        _owner = context.Owner;
        AudioManager.Instance.PlaySFXAt(_definition.EquipSFX, _owner.transform);
    }
    protected virtual void OnArmorWornOut()
    {
        ArmorWornOut?.Invoke(_definition);
        AudioManager.Instance.PlaySFXAt(_definition.UnequipSFX, _owner.transform);
    }

    protected void OnItemUsageFinished()
    {
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
