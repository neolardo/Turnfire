using System.Collections.Generic;

public abstract class ArmorDefinition : ItemDefinition
{
    public override ItemType ItemType => ItemType.Armor;

    public RangedStatInt MaxDurability;
    public abstract bool IsProtective { get; }

    public override IEnumerable<RangedStat> GetRangedStats()
    {
        return new[] { MaxDurability };
    }

}
