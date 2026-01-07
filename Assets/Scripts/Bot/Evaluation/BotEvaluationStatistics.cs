using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class BotEvaluationStatistics
{
    private static readonly Dictionary<Team, BotEvaluationData> _dataPerTeam = new Dictionary<Team, BotEvaluationData>();
    private static readonly Dictionary<Team, BotDifficulty> _difficultyPerTeam = new Dictionary<Team, BotDifficulty>();

    private static string LogFolder = null;
    private static string GetFilePath(BotEvaluationConfiguration config) => Path.Combine(LogFolder, $"bot_evaluation_{OfflineSceneLoader.Instance.CurrentGameplaySceneSettings.Map.SceneName}_{config}.csv");

    public static int CurrentSimulationCount { get; private set; }
    private static int _requestedSimulationCount = 100;

    private static int _turnCount;
    private const int LiveLockTurnCountThreshold = 100;


    public static void RegisterBot(Team team, BotDifficulty difficulty)
    {
        _dataPerTeam[team] = new BotEvaluationData();
        _dataPerTeam[team].TeamName = team.TeamName;
        _difficultyPerTeam[team] = difficulty;
        team.TeamName += $" {difficulty}";
    } 

    public static BotEvaluationData GetData(Team team)
    {
        return _dataPerTeam[team];
    }

    public static IEnumerable<BotEvaluationData> GetAllData()
    {
        return _dataPerTeam.Values;
    }

    public static void OnTurnFinished()
    {
        _turnCount++;
        if(_turnCount >= LiveLockTurnCountThreshold )
        {
            ForceEndLiveLockedRound();
        }
    }

    private static void ForceEndLiveLockedRound()
    {
        Debug.Log("Round force ended because of possible live lock");
        GameServices.TurnStateManager.ForceEndGame();
    }

    public static void Clear()
    {
        _dataPerTeam.Clear();
        _difficultyPerTeam.Clear();
        _turnCount = 0;
    }

    public static void Save()
    {
        var teams = _dataPerTeam.Keys;
        foreach(var team in teams)
        {
            var analyzed = team;
            var other = teams.Where(t=> t != analyzed).First();
            if (_difficultyPerTeam[other] == _difficultyPerTeam[analyzed])
            {
                continue;
            }

            var config = new BotEvaluationConfiguration(_difficultyPerTeam[analyzed], _difficultyPerTeam[other]);
            AppendToFile(config, _dataPerTeam[analyzed]);
        }
        CurrentSimulationCount++;
    }

    public static void TryToRestartSimulation()
    {
        if(CurrentSimulationCount <  _requestedSimulationCount)
        {
            Clear();
            OfflineSceneLoader.Instance.ReloadScene();
        }
        else 
        {
            Application.Quit();
        }
    }


    private static void AppendToFile(BotEvaluationConfiguration config, BotEvaluationData data)
    {
        var filePath = GetFilePath(config);
        CreateHeaderIfNeeded(filePath);
        File.AppendAllText(filePath, data.ToString());
    }

    private static void CreateHeaderIfNeeded(string filePath)
    {
        if (File.Exists(filePath)) 
            return;

        File.WriteAllText(filePath, string.Join(",", BotEvaluationData.Headers) + "\n");
    }
}
