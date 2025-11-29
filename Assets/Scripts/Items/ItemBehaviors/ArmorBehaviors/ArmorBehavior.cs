using System;

public abstract class ArmorBehavior : IItemBehavior
{
    protected int _durability;
    protected Character _owner;
    public bool IsInUse {get; protected set;}

    public event Action ItemUsageFinished;

    public void InitializePreview(ItemUsageContext context, ItemPreviewRendererManager rendererManager)
    {
        //TODO
    }

    public virtual void Use(ItemUsageContext context)
    {
        IsInUse = true;
        _owner = context.Owner;
    }

    protected abstract void OnOwnerDied();
    protected abstract void OnArmorWornOut();

    protected void InvokeItemUsageFinished()
    {
        IsInUse = false;
        ItemUsageFinished?.Invoke();
    }

    protected void DecreaseDurability()
    {
        _durability--;
        if(_durability == 0 ) 
        {
            OnArmorWornOut();
        }
    }

}
