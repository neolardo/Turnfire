using System.Collections.Generic;
using UnityEngine;

public abstract class ItemDefinition : CollectibleDefinition
{
    public override CollectibleType Type => CollectibleType.Item;

    public Sprite Sprite;
    public string Name;
    public string Description;
    public AnimationDefinition ItemActionAnimationDefinition;
    public abstract IItemBehavior CreateItemBehavior();
    public abstract IEnumerable<RangedStat> GetRangedStats();

}
