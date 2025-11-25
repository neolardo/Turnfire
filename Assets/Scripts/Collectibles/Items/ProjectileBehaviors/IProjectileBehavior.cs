using System;
using System.Collections.Generic;
using UnityEngine;
public interface IProjectileBehavior
{
    public event Action<ExplosionInfo> Exploded;
    public void Launch(ProjectileLaunchContext context);
    public void OnContact(ProjectileContactContext context);
    public void SetProjectile(Projectile projectile);
    public void ForceExplode();
    public WeaponBehaviorSimulationResult SimulateProjectileBehavior(Vector2 start, Vector2 aimVector, DestructibleTerrainManager destructibleTerrain, Character owner, IEnumerable<Character> others);
}
