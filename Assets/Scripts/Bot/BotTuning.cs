[System.Serializable]
public class BotTuning
{
    // personality

    // movement weights
    public float SafePlaceSearchHealthThreshold;
    public float SafePlaceSearchWeight;
    public float PackageSearchWeight;
    public float BestDamagingPlaceSearchWeight;

    // attack weights
    public float DamageDealtWeight;
    public float FriendlyFireDamageDealtWeight;
    public float RemainingAmmoWeight;
    public bool PreferHighestDamageDealingWeapon;

    // skills
    public float AimRandomnessBias;

    // noise
    public float DecisionRandomnessBias;
}
