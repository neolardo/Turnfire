using System.Collections.Generic;
using UnityEngine;

public readonly struct ItemBehaviorSimulationContext
{
    public readonly Character Owner;
    public readonly IEnumerable<Character> OtherCharacters;
    public readonly Vector2 Origin;
    public readonly Vector2 AimVector;
    public readonly TerrainManager Terrain;

    public ItemBehaviorSimulationContext(Character owner, IEnumerable<Character> otherCharacters, Vector2 origin, Vector2 aimVector, TerrainManager terrain )
    {
        Owner = owner;
        OtherCharacters = otherCharacters;
        Origin = origin;
        AimVector = aimVector;
        Terrain = terrain;
    }
    public ItemBehaviorSimulationContext(ItemBehaviorSimulationContext context, float aimMultiplier)
    {
        Owner = context.Owner;
        OtherCharacters = context.OtherCharacters;
        Origin = context.Origin;
        AimVector = context.AimVector * aimMultiplier;
        Terrain = context.Terrain;
    }

}
