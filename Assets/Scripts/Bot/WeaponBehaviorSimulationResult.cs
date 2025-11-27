using UnityEngine;

public readonly struct WeaponBehaviorSimulationResult
{
    public readonly float TotalDamageDealtToEnemies;
    public readonly float TotalDamageDealtToAllies;
    public readonly Vector2 DamageCenter;
    public static WeaponBehaviorSimulationResult Zero => new WeaponBehaviorSimulationResult(Vector2.zero, 0,0);

    public WeaponBehaviorSimulationResult(Vector2 center, float enemyDamage, float allyDamage)
    {
        DamageCenter = center;
        TotalDamageDealtToEnemies = enemyDamage;
        TotalDamageDealtToAllies = allyDamage;
    }
}
