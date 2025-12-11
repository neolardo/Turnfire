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
        }

        //CreateRandomizedBotEvaluationTeamSetup(BotDifficulty.Easy, BotDifficulty.Medium);
        CreateRandomizedTeamSetup();
    }

    private void CreateRandomizedTeamSetup()
    {
        //TODO: local multiplayer
        //TODO: remote players
        var botManagerFactory = FindFirstObjectByType<BotManagerFactory>();
        var playerNames = SceneLoader.Instance.CurrentGameplaySceneSettings.PlayerNames;
        int playerCount = 0;
        int botCount = 0;
        Team localTeam = _teams[Random.Range(0, _teams.Count)];
        localTeam.InitializeInputSource(InputSourceType.Local);
        localTeam.TeamName = playerNames[playerCount++]; //TODO: local player's name
        foreach (var team in _teams)
        {
            if (team == localTeam)
                continue;

            team.InitializeInputSource(InputSourceType.Bot);
            team.TeamName = Constants.DefaultBotName + $"{botCount+1}";
            botCount++;
            botManagerFactory.CreateBotForTeam(team, SceneLoader.Instance.CurrentGameplaySceneSettings.BotDifficulty);
        }
    }

    private void CreateRandomizedBotEvaluationTeamSetup(BotDifficulty analyzedDifficulty, BotDifficulty otherDifficulty)
    {
        var botManagerFactory = FindFirstObjectByType<BotManagerFactory>();
        Team analyzedTeam = _teams[Random.Range(0, _teams.Count)];
        analyzedTeam.InitializeInputSource(InputSourceType.Bot);
        botManagerFactory.CreateBotForTeam(analyzedTeam, analyzedDifficulty);
        foreach (var team in _teams)
        {
            if (team == analyzedTeam)
                continue;

            team.InitializeInputSource(InputSourceType.Bot);
            botManagerFactory.CreateBotForTeam(team, otherDifficulty);
        }
    }

}
