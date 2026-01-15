using System;
using UnityEngine;

public class OfflineTerrainExplosionRegistry : MonoBehaviour, ITerrainExplosionRegistry
{
    public event Action<Vector2, float> ExplosionRegistered;

    private void Awake()
    {
        var terrain = FindFirstObjectByType<TerrainManager>();
        terrain.SetExplosionRegistry(this);
    }

    public void RegisterExplosion(Vector2 worldPos, float radius)
    {
        ExplosionRegistered?.Invoke(worldPos, radius);
    }
}
