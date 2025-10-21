using System.Collections.Generic;
using UnityEngine;

public abstract class ItemDefinition : CollectibleDefinition
{
    public override CollectibleType CollectibleType => CollectibleType.Item;
    public abstract ItemType ItemType { get; }

    public Sprite Sprite;
    public string Name;
    public string Description;
    public AnimationDefinition ItemActionAnimation;
    public SFXDefiniton ItemActionSFX;
    public bool HideItemDuringUsage;
    public abstract IItemBehavior CreateItemBehavior();
    public abstract IEnumerable<RangedStat> GetRangedStats();

}
