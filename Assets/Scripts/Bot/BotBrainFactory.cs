using UnityEngine;

public class BotBrainFactory : MonoBehaviour
{
    [SerializeField] private BotTuning _easyTuning;
    [SerializeField] private BotTuning _mediumTuning;
    [SerializeField] private BotTuning _hardTuning;
    public BotBrain CreateBrain(BotDifficulty difficulty, BotGameplayInput input)
    {
        switch (difficulty)
        {
            case BotDifficulty.Easy:
                return new BotBrain(_easyTuning, input);
            case BotDifficulty.Medium:
                return new BotBrain(_mediumTuning, input);
            case BotDifficulty.Hard:
                return new BotBrain(_hardTuning, input);
            default:
                throw new System.Exception($"Invalid {nameof(BotDifficulty)} when creating {nameof(BotBrain)}s: {difficulty}");
        }

    }
}
