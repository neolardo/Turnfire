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
        GameServices.TurnStateManager.Initialize(_teams);
    }

    private void InitializeTeams()
    {
        int playerCount = GameplaySceneSettingsStorage.Current.Players.Count;
        _teams = _possibleTeams.Take(playerCount).ToList();
        for (int i = _teams.Count; i < _possibleTeams.Count; i++)
        {
            _possibleTeams[i].gameObject.SetActive(false);
        }
        CreateRandomizedTeamSetup();
    }

    private void CreateRandomizedTeamSetup()
    {
        var botManagerFactory = FindFirstObjectByType<BotManagerFactory>();
        var gameplaySettings = GameplaySceneSettingsStorage.Current;
        var players = gameplaySettings.Players;
        foreach (var player in players) 
        {
            var team = _teams[player.TeamIndex];
            team.TeamName = player.Name;
            if (player.Type == PlayerType.Human)
            {
                team.InitializeInputSource(InputSourceType.Local); //TODO: remote?
            }
            else if(player.Type == PlayerType.Bot)
            {
                team.InitializeInputSource(InputSourceType.Bot);
                botManagerFactory.CreateBotForTeam(team, gameplaySettings.BotDifficulty);
            }
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
