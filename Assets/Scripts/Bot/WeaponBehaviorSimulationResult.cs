public readonly struct WeaponBehaviorSimulationResult
{
    public readonly float TotalDamageDealtToEnemies;
    public readonly float TotalDamageDealtToAllies;

    public WeaponBehaviorSimulationResult(float damageToEnemies, float damageToAllies)
    {
        TotalDamageDealtToEnemies = damageToEnemies;
        TotalDamageDealtToAllies = damageToAllies;
    }
}
