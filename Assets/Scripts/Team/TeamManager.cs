using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
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
        int playerCount = GameplaySceneSettingsStorage.Current.Players.Count;
        _teams = _possibleTeams.Take(playerCount).ToList();
        for (int i = _teams.Count; i < _possibleTeams.Count; i++)
        {
            _possibleTeams[i].gameObject.SetActive(false);
        }
        StartCoroutine(CreateTeamSetupCoroutine());
    }

    private IEnumerator CreateTeamSetupCoroutine()
    {
        var botManagerFactory = FindFirstObjectByType<BotManagerFactory>();

        var settings = GameplaySceneSettingsStorage.Current;
        var players = settings.Players;
        var isOnlineGame = settings.IsOnlineGame;
        foreach (var player in players)
        {
            var team = _teams[player.TeamIndex];
            ITeamInputSource inputSource;

            if (!isOnlineGame)
            {
                inputSource = TeamInputSourceFactory.Create(player.Type == PlayerType.Human ? InputSourceType.OfflineHuman : InputSourceType.OfflineBot, team.transform);
            }
            else
            {
                inputSource = TeamInputSourceFactory.Create(player.Type == PlayerType.Human? InputSourceType.OnlineHuman : InputSourceType.OnlineBot,team.transform);
                var netObj = (inputSource as NetworkBehaviour).GetComponent<NetworkObject>();
                netObj.SpawnWithOwnership(player.ClientId, true);
                yield return new WaitUntil(() => netObj.IsSpawned);
            }

            team.Initialize(inputSource, player.TeamIndex, player.Name);

            if (player.Type == PlayerType.Bot)
            {
                botManagerFactory.CreateBotForTeam(team, settings.BotDifficulty);
            }
        }
        GameServices.TurnStateManager.Initialize(_teams);
    }


    private void CreateRandomizedBotEvaluationTeamSetup(BotDifficulty analyzedDifficulty, BotDifficulty otherDifficulty)
    {
        var botManagerFactory = FindFirstObjectByType<BotManagerFactory>();
        int teamIndex = Random.Range(0, _teams.Count);
        Team analyzedTeam = _teams[teamIndex];
        var analyzedTeamInput = TeamInputSourceFactory.Create(InputSourceType.OfflineBot, analyzedTeam.transform);
        analyzedTeam.Initialize(analyzedTeamInput, teamIndex, analyzedDifficulty.ToString() );
        botManagerFactory.CreateBotForTeam(analyzedTeam, analyzedDifficulty);
        foreach (var team in _teams)
        {
            if (team == analyzedTeam)
                continue;
            teamIndex = _teams.IndexOf(team);
            var otherTeamInput = TeamInputSourceFactory.Create(InputSourceType.OfflineBot, analyzedTeam.transform);
            team.Initialize(otherTeamInput, teamIndex, otherDifficulty.ToString());
            botManagerFactory.CreateBotForTeam(team, otherDifficulty);
        }
        GameServices.TurnStateManager.Initialize(_teams);
    }

}
