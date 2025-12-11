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

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        if (obj is null) return false;
        if (obj.GetType() != GetType()) return false;

        var other = (BotEvaluationConfiguration)obj;
        return other == this;
    }

    public override int GetHashCode()
    {
        unchecked 
        {
            int hash = 17;
            hash = hash * 23 + AnalyzedBotDifficulty.GetHashCode();
            hash = hash * 23 + OtherBotDifficulty.GetHashCode();
            return hash;
        }
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
