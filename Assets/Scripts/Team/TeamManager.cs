using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    [SerializeField] private List<Team> _possibleTeams;
    private List<Team> _teams;

    private void Start()
    {
        if (_possibleTeams == null || _possibleTeams.Count == 0)
        {
            Debug.LogWarning("There are no teams.");
        }

        InitializeTeams();
        var turnManager = FindFirstObjectByType<TurnManager>();
        turnManager.Initialize(_teams);
    }

    private void InitializeTeams()
    {
        _teams = _possibleTeams.Take(SceneLoader.Instance.CurrentGameplaySceneSettings.NumTeams).ToList();
        for (int i = _teams.Count; i < _possibleTeams.Count; i++)
        {
            _possibleTeams[i].gameObject.SetActive(false);
            Debug.Log(_possibleTeams[i].TeamName + " disabled");
        }
        var firstBotArg = System.Environment.GetCommandLineArgs().First(a => a.StartsWith("-bot-a-")).Substring(7);
        var secondBotArg = System.Environment.GetCommandLineArgs().First(a => a.StartsWith("-bot-b-")).Substring(7);
        CreateRandomizedBotEvaluationTeamSetup((BotDifficulty)int.Parse(firstBotArg), (BotDifficulty)int.Parse(secondBotArg));
       //CreateRandomizedTeamSetup();
    }



    private void CreateRandomizedTeamSetup()
    {
        //TODO: remote players
        var botManagerFactory = FindFirstObjectByType<BotManagerFactory>();
        Team localTeam = _teams[UnityEngine.Random.Range(0, _teams.Count)];
        localTeam.InitializeInputSource(InputSourceType.Local);
        foreach(var team in _teams)
        {
            if (team == localTeam)
                continue;

            team.InitializeInputSource(InputSourceType.Bot);
            botManagerFactory.CreateBotForTeam(team, SceneLoader.Instance.CurrentGameplaySceneSettings.BotDifficulty);
        }
    }

    private void CreateRandomizedBotEvaluationTeamSetup(BotDifficulty analyzedDifficulty, BotDifficulty otherDifficulty)
    {
        Debug.Log("Team setup creation started");
        var botManagerFactory = FindFirstObjectByType<BotManagerFactory>();
        Team analyzedTeam = _teams[UnityEngine.Random.Range(0, _teams.Count)];
        analyzedTeam.InitializeInputSource(InputSourceType.Bot);
        botManagerFactory.CreateBotForTeam(analyzedTeam, analyzedDifficulty);
        BotEvaluationStatistics.RegisterBot(analyzedTeam, analyzedDifficulty);
        foreach (var team in _teams)
        {
            if (team == analyzedTeam)
                continue;

            team.InitializeInputSource(InputSourceType.Bot);
            botManagerFactory.CreateBotForTeam(team, otherDifficulty);
            BotEvaluationStatistics.RegisterBot(team, otherDifficulty);
        }
    }

}
