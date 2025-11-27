using UnityEngine;

[CreateAssetMenu(fileName = "BotTuningDefinition", menuName = "Scriptable Objects/BotTuningDefinition")]

public class BotTuningDefinition : ScriptableObject
{
    [Header("Movement")]
    public float OffensivePositionPreference;
    public float DefensivePositionPreference;
    public float TravelDistanceSensitivity;
    public float LowHealhThreshold;

    public float GeneralPackageSearchWeight;
    public float RemainingAmmoLowPackageSearchWeight;
    public int RemainingAmmoLowThreshold;
    public float OutOfAmmoPackageSearchWeight;

    [Header("Attack")]
    public float DamageDealtToEnemiesWeight;
    public float DamageDealtToAlliesWeight;
    public float DamageCenterDistanceFromClosestEnemyWeight;
    public bool PreferHighestDamageDealingWeapon;
    public bool OnlyShootIfCanDealDamage;

    [Header("Skills")]
    public float AimRandomnessBias;

    [Header("Noise")]
    public float DecisionRandomnessBias;
}
