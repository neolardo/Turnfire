using System;

public abstract class ConsumableBehavior : IItemBehavior
{
    public bool IsInUse { get;protected set; }

    public event Action ItemUsageFinished;

    public virtual bool CanUseItem(ItemUsageContext context)
    {
        return true;
    }

    public virtual void InitializePreview(ItemUsageContext context, ItemPreviewRendererManager rendererManager)
    {
        //TODO
    }

    public abstract void Use(ItemUsageContext context);

    protected void InvokeItemUsageFinished()
    {
        IsInUse = false;
        ItemUsageFinished?.Invoke();
    }
}
