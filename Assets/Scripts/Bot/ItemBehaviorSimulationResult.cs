using UnityEngine;

public readonly struct ItemBehaviorSimulationResult
{
    public readonly float TotalDamageDealtToEnemies;
    public readonly float TotalDamageDealtToAllies;
    public readonly Vector2 DamageCenter;
    public readonly float TotalHealingDone;
    public readonly int TotalArmorBoost;
    public readonly float TotalMobilityBoost;

    public static ItemBehaviorSimulationResult None => new ItemBehaviorSimulationResult(Vector2.zero, 0, 0, 0, 0, 0);
    public static ItemBehaviorSimulationResult ArmorBoost(int armorBoost) => new ItemBehaviorSimulationResult(Vector2.zero, 0, 0, 0, armorBoost, 0);
    public static ItemBehaviorSimulationResult MobilityBoost(float mobilityBoost) => new ItemBehaviorSimulationResult(Vector2.zero, 0, 0, 0, 0, mobilityBoost);
    public static ItemBehaviorSimulationResult Healing(float healing) => new ItemBehaviorSimulationResult(Vector2.zero, 0, 0, healing, 0, 0);
    public static ItemBehaviorSimulationResult Damage(Vector2 center, float enemyDamage, float allyDamage) => new ItemBehaviorSimulationResult(center, enemyDamage, allyDamage, 0, 0, 0);
    
    public ItemBehaviorSimulationResult(Vector2 center, float enemyDamage, float allyDamage, float healing, int armorBoost, float mobilityBoost)
    {
        DamageCenter = center;
        TotalDamageDealtToEnemies = enemyDamage;
        TotalDamageDealtToAllies = allyDamage;
        TotalHealingDone = healing;
        TotalArmorBoost = armorBoost;
        TotalMobilityBoost = mobilityBoost;
    }
}
