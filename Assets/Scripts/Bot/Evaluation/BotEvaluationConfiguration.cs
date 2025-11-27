using System;

public struct BotEvaluationConfiguration
{
    public BotDifficulty AnalyzedBotDifficulty;
    public BotDifficulty OtherBotDifficulty;

    public BotEvaluationConfiguration(BotDifficulty analyzedBotDifficulty, BotDifficulty otherBotDifficulty)
    {
        AnalyzedBotDifficulty = analyzedBotDifficulty;
        OtherBotDifficulty = otherBotDifficulty;
    }

    public override string ToString()
    {
        return $"analyzed_{Enum.GetName(typeof(BotDifficulty), AnalyzedBotDifficulty)}_vs_other_{Enum.GetName(typeof(BotDifficulty), OtherBotDifficulty)}"; 
    }

    public static bool operator==(BotEvaluationConfiguration a, BotEvaluationConfiguration b)
    {
        return a.AnalyzedBotDifficulty == b.AnalyzedBotDifficulty && a.OtherBotDifficulty == b.OtherBotDifficulty;
    }

    public static bool operator!=(BotEvaluationConfiguration a, BotEvaluationConfiguration b)
    {
        return !(a == b);
    }

}
