using System;
using System.Collections;

public interface IItemBehavior
{
    public event Action ItemUsageFinished;
    public bool IsInUse { get; }
    public void Use(ItemUsageContext context);
    public bool CanUseItem(ItemUsageContext context);
    public void InitializePreview(ItemUsageContext context, ItemPreviewRendererManager rendererManager);
    public IEnumerator SimulateUsage(ItemBehaviorSimulationContext context, Action<ItemBehaviorSimulationResult> onDone);
}
