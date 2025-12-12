public class BotEvaluationData
{
    public string TeamName; 
    public BotEvaluationRoundResult RoundResult; 
    public float TotalDamageDealtToEnemies;
    public float TotalDamageDealtToAllies;
    public int TotalSuicideCount; 
    public int TotalNonDamagingAttackCount; 
    public float RemainingNormalizedTeamHealth;
    public int TotalTurnCount; 
    public int TotalSkippedMovementCount; 
    public int TotalOpenedPackageCount;
    public float TotalArmorsEquipped;
    public float TotalConsumablesUsed;


    public override string ToString()
    {
        return $"{System.DateTime.Now:yyyy-MM-dd HH:mm:ss}," +
            $"{TeamName}," +
            $"{RoundResult}," +
            $"{TotalDamageDealtToEnemies}," +
            $"{TotalDamageDealtToAllies}," +
            $"{TotalSuicideCount}," +
            $"{TotalNonDamagingAttackCount}," +
            $"{RemainingNormalizedTeamHealth}," +
            $"{TotalTurnCount}," +
            $"{TotalSkippedMovementCount}," +
            $"{TotalOpenedPackageCount},"+
            $"{TotalArmorsEquipped},"+
            $"{TotalConsumablesUsed}\n";
    }

    public static string[] Headers => new[]
    {
        "Timestamp",
        nameof(TeamName),
        nameof(RoundResult),
        nameof(TotalDamageDealtToEnemies), 
        nameof(TotalDamageDealtToAllies), 
        nameof(TotalSuicideCount),
        nameof(TotalNonDamagingAttackCount),
        nameof(RemainingNormalizedTeamHealth),
        nameof(TotalTurnCount),
        nameof(TotalSkippedMovementCount),
        nameof(TotalOpenedPackageCount),
        nameof(TotalArmorsEquipped),
        nameof(TotalConsumablesUsed)
    };

    public void Clear()
    {
        TeamName = string.Empty;
        RoundResult = BotEvaluationRoundResult.None;
        TotalDamageDealtToEnemies = 0;
        TotalDamageDealtToAllies = 0;
        TotalSuicideCount = 0;
        TotalNonDamagingAttackCount = 0;
        RemainingNormalizedTeamHealth = 0;
        TotalTurnCount = 0;
        TotalSkippedMovementCount = 0;
        TotalOpenedPackageCount = 0;
        TotalArmorsEquipped = 0;
        TotalConsumablesUsed = 0;
    }
}
