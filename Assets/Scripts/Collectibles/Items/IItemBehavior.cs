public interface IItemBehavior
{
    public bool IsInUse { get; }
    public void Use(ItemUsageContext context);
    public void InitializePreview(ItemUsageContext context, ItemPreviewRendererManager rendererManager);
}
