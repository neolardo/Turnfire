using System.Collections.Generic;
using UnityEngine;

public abstract class ItemDefinition : DatabaseItemScriptableObject
{
    public abstract ItemType ItemType { get; }

    public Sprite Sprite;
    public string Name;
    public string Description;
    public int InitialQuantity;
    public int MaximumQuantity;
    public bool IsQuantityInfinite;
    public RangedStatInt DropQuantityRange;
    public AnimationDefinition ItemActionAnimation;
    public SFXDefiniton ItemActionSFX;
    public float ItemUsagePostDelay;
    public bool HideItemDuringUsage;
    public bool UseInstantlyWhenSelected;

    public abstract IItemBehavior CreateItemBehavior();
    public abstract IEnumerable<RangedStat> GetRangedStats();

}
