using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    [SerializeField] private List<Team> _possibleTeams;
    private Team[] _teams;

    public bool AllTeamsInitialized => _teams == null ? false : _teams.All(t=> t.IsTeamInitialized);

    private void Start()
    {
        ActvatePlayableTeams();
        InitializeTeams();
    }

    private void ActvatePlayableTeams()
    {
        if (_possibleTeams == null || _possibleTeams.Count == 0)
        {
            Debug.LogWarning("There are no teams.");
        }
        int playerCount = GameplaySceneSettingsStorage.Current.Players.Count;
        _teams = _possibleTeams.Take(playerCount).ToArray();
        for (int i = _teams.Length; i < _possibleTeams.Count; i++)
        {
            _possibleTeams[i].gameObject.SetActive(false);
        }
    }

    private void InitializeTeams()
    {
        var botManagerFactory = FindFirstObjectByType<BotManagerFactory>();
        var settings = GameplaySceneSettingsStorage.Current;
        var players = settings.Players;
        foreach (var player in players)
        {
            var team = _teams[player.TeamIndex];
            TeamComposer.Compose(team, player, botManagerFactory);
            Debug.Log($"team {team.TeamName} composed");
        }
    }


    public IEnumerable<Team> GetTeams()
    {
        return _teams;
    }

}
