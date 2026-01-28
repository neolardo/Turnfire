using System;
using UnityEngine;

public interface ITerrainExplosionRegistry
{
    public event Action<Vector2, float> ExplosionRegistered;
    public void RegisterExplosion(Vector2 worldPos, float radius);
}
