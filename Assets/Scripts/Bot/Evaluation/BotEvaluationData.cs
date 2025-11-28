public class BotEvaluationData
{
    public bool HasWon; 
    public float TotalDamageDealtToEnemies;
    public float TotalDamageDealtToAllies;
    public int TotalSuicideCount; 
    public int TotalNonDamagingProjectileCount; 
    public float RemainingNormalizedTeamHealth;
    public int TotalTurnCount; 
    public int TotalSkippedTurnCount; 
    public int TotalOpenedPackageCount;

    public override string ToString()
    {
        return $"{System.DateTime.Now:yyyy-MM-dd HH:mm:ss}," +
            $"{HasWon}," +
            $"{TotalDamageDealtToEnemies}," +
            $"{TotalDamageDealtToAllies}," +
            $"{TotalSuicideCount}," +
            $"{TotalNonDamagingProjectileCount}," +
            $"{RemainingNormalizedTeamHealth}," +
            $"{TotalTurnCount}," +
            $"{TotalSkippedTurnCount}," +
            $"{TotalOpenedPackageCount}\n";
    }

    public static string[] Headers => new[]
    {
        "Timestamp", 
        nameof(HasWon),
        nameof(TotalDamageDealtToEnemies), 
        nameof(TotalDamageDealtToAllies), 
        nameof(TotalSuicideCount),
        nameof(TotalNonDamagingProjectileCount),
        nameof(RemainingNormalizedTeamHealth),
        nameof(TotalTurnCount),
        nameof(TotalSkippedTurnCount),
        nameof(TotalOpenedPackageCount)
    };

    public void Clear()
    {
        HasWon = false;
        TotalDamageDealtToEnemies = 0;
        TotalDamageDealtToAllies = 0;
        TotalSuicideCount = 0;
        TotalNonDamagingProjectileCount = 0;
        RemainingNormalizedTeamHealth = 0;
        TotalTurnCount = 0;
        TotalSkippedTurnCount = 0;
        TotalOpenedPackageCount = 0;
    }
}
