using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class BotEvaluationStatistics
{
    private static Dictionary<Team, BotEvaluationData> _dataPerTeam;
    private static Dictionary<Team, BotDifficulty> _difficultyPerTeam;
    private static string GetFilePath(BotEvaluationConfiguration config) => Path.Combine(Application.persistentDataPath, $"bot_evaluation_result_{SceneLoader.Instance.CurrentGameplaySceneSettings.SceneName}_{config}.csv");

    public static void RegisterBot(Team team, BotDifficulty difficulty)
    {
        _dataPerTeam[team] = new BotEvaluationData();
        _difficultyPerTeam[team] = difficulty;
    } 

    public static BotEvaluationData GetData(Team team)
    {
        return _dataPerTeam[team];
    }

    public static void Clear()
    {
        foreach (var data in _dataPerTeam.Values)
        {
            data.Clear();
        }
    }

    public static void Save()
    {
        var teams = _dataPerTeam.Keys;
        foreach(var team in teams)
        {
            var analyzed = team;
            var other = teams.Where(t=> t != analyzed).First();
            var config = new BotEvaluationConfiguration(_difficultyPerTeam[analyzed], _difficultyPerTeam[other]);
            AppendToFile(config, _dataPerTeam[analyzed]);
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
