using System.Collections.Generic;
using UnityEngine;

public readonly struct ItemBehaviorSimulationContext
{
    public readonly Character Owner;
    public readonly IEnumerable<Character> OtherCharacters;
    public readonly Vector2 Origin;
    public readonly Vector2 AimVector;
    public readonly DestructibleTerrainManager Terrain;

    public ItemBehaviorSimulationContext(Character owner, IEnumerable<Character> otherCharacters, Vector2 origin, Vector2 aimVector, DestructibleTerrainManager terrain )
    {
        Owner = owner;
        OtherCharacters = otherCharacters;
        Origin = origin;
        AimVector = aimVector;
        Terrain = terrain;
    }

}
