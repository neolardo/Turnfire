public class BotEvaluationData
{
    public string TeamName; 
    public BotEvaluationRoundResult RoundResult; 
    public float DamageDealtToEnemies;
    public float DamageDealtToAllies;
    public int SuicideCount; 
    public int NonDamagingAttackCount; 
    public float RemainingNormalizedTeamHealth;
    public int TotalTurnCount; 
    public int SkippedMovementCount; 
    public int OpenedPackageCount;
    public float ArmorsEquippedCount;
    public float ConsumablesUsedCount;


    public override string ToString()
    {
        return $"{System.DateTime.Now:yyyy-MM-dd HH:mm:ss}," +
            $"{TeamName}," +
            $"{RoundResult}," +
            $"{DamageDealtToEnemies}," +
            $"{DamageDealtToAllies}," +
            $"{SuicideCount}," +
            $"{NonDamagingAttackCount}," +
            $"{RemainingNormalizedTeamHealth}," +
            $"{TotalTurnCount}," +
            $"{SkippedMovementCount}," +
            $"{OpenedPackageCount},"+
            $"{ArmorsEquippedCount},"+
            $"{ConsumablesUsedCount}\n";
    }

    public static string[] Headers => new[]
    {
        "Timestamp",
        nameof(TeamName),
        nameof(RoundResult),
        nameof(DamageDealtToEnemies), 
        nameof(DamageDealtToAllies), 
        nameof(SuicideCount),
        nameof(NonDamagingAttackCount),
        nameof(RemainingNormalizedTeamHealth),
        nameof(TotalTurnCount),
        nameof(SkippedMovementCount),
        nameof(OpenedPackageCount),
        nameof(ArmorsEquippedCount),
        nameof(ConsumablesUsedCount)
    };

    public void Clear()
    {
        TeamName = string.Empty;
        RoundResult = BotEvaluationRoundResult.None;
        DamageDealtToEnemies = 0;
        DamageDealtToAllies = 0;
        SuicideCount = 0;
        NonDamagingAttackCount = 0;
        RemainingNormalizedTeamHealth = 0;
        TotalTurnCount = 0;
        SkippedMovementCount = 0;
        OpenedPackageCount = 0;
        ArmorsEquippedCount = 0;
        ConsumablesUsedCount = 0;
    }
}
