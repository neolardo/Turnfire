using System;
using UnityEngine;

public class BotManagerFactory : MonoBehaviour
{
    [Header("Bot Tunings per Difficulty")]
    [SerializeField] private BotTuningDefinition _easyTuning;
    [SerializeField] private BotTuningDefinition _mediumTuning;
    [SerializeField] private BotTuningDefinition _hardTuning;

    public void CreateBotForTeam(Team team, BotDifficulty difficulty)
    {
        var input = team.InputSource as BotGameplayInput;
        var brain = CreateBrain(difficulty);
        var botManager = team.gameObject.AddComponent<BotManager>();
        botManager.Initialize(team, brain, input);
        BotEvaluationStatistics.RegisterBot(team, difficulty);
        BotEvaluationStatistics.GetData(team).TeamName = team.TeamName;
    }

    private BotBrain CreateBrain(BotDifficulty difficulty)
    {
        BotTuningDefinition tuning = default;
        switch (difficulty)
        {
            case BotDifficulty.Easy:
                tuning = _easyTuning;
                break;
            case BotDifficulty.Medium:
                tuning = _mediumTuning;
                break;
            case BotDifficulty.Hard:
                tuning = _hardTuning;
                break;
            default:
                throw new Exception($"Invalid {nameof(BotDifficulty)} when creating {nameof(BotBrain)}s: {difficulty}");
        }
        return new BotBrain(tuning, this);
    }
}
