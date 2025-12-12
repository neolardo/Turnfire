using UnityEngine;

[CreateAssetMenu(fileName = "BotTuningDefinition", menuName = "Scriptable Objects/BotTuningDefinition")]

public class BotTuningDefinition : ScriptableObject
{
    [Header("Personality")]
    public float Offense;
    public float Defense;
    public float MobilityPreference;
    public float TravelDistanceSensitivity;
    public float TravelDistanceWeight;
    public float LowHealthThreshold;

    public int RemainingAmmoLowThreshold;

    public float GeneralPackageGreed;
    public float RemainingAmmoLowPackageGreed;
    public float OutOfAmmoPackageGreed;

    [Header("Skills")]
    public float AimRandomnessBias;

    [Header("Noise")]
    public float DecisionRandomnessBias;
}
