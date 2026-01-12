using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class OnlineTeamComposer : NetworkBehaviour, ITeamComposer
{
    public bool AllTeamsInitialized { get; private set; }

    private NetworkVariable<bool> _allInputSourcesCreated;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _allInputSourcesCreated.OnValueChanged += OnAllInputSourcesCreatedChanged;
    }

    public override void OnNetworkDespawn()
    {
        _allInputSourcesCreated.OnValueChanged -= OnAllInputSourcesCreatedChanged;
        base.OnNetworkDespawn();
    }

    public void ComposeTeams(Team[] teams)
    {
        if(!IsServer)
        {
            return;
        }
        CreateInputSources(teams);
    }

    private void CreateInputSources(Team[] teams)
    {
        if (!IsServer)
        {
            return;
        }
        var settings = GameplaySceneSettingsStorage.Current;
        var players = settings.Players;
        var netObjects = new List<NetworkObject>();
        foreach (var player in players)
        {
            var team = teams[player.TeamIndex];
            var inputSource = TeamInputSourceFactory.Create(player.Type == PlayerType.Human ? InputSourceType.OnlineHuman : InputSourceType.OnlineBot, team.transform);
            var netObj = (inputSource as NetworkBehaviour).GetComponent<NetworkObject>();
            netObj.SpawnAsPlayerObject(player.ClientId, true);
            netObjects.Add(netObj);
        }
        StartCoroutine(WaitForAllInputSourcesToBeSpawned(netObjects));
    }

    private IEnumerator WaitForAllInputSourcesToBeSpawned(IEnumerable<NetworkObject> netObjects)
    {
        foreach (var netObject in netObjects)
        {
            yield return new WaitUntil(() => netObject.IsSpawned);
        }
        _allInputSourcesCreated.Value = true;
    }

    private void OnAllInputSourcesCreatedChanged(bool previousValue, bool newValue)
    {
        if(newValue)
        {
            InitializeTeams();
        }
    }

    private void InitializeTeams()
    {
        var teamComposerBootstrap = FindFirstObjectByType<TeamComposerBootstrap>();
        var teams = teamComposerBootstrap.GetTeams().ToArray();

        var botManagerFactory = FindFirstObjectByType<BotManagerFactory>();
        var settings = GameplaySceneSettingsStorage.Current;
        var players = settings.Players;
        foreach (var player in players)
        {
            var team = teams[player.TeamIndex];
            ITeamInputSource inputSource = null;
            if (player.Type == PlayerType.Human)
            {
                var possibleSources = FindObjectsByType<OnlineHumanTeamInputSource>(FindObjectsSortMode.InstanceID);
                inputSource = possibleSources.First( s => s.GetComponent<NetworkObject>().OwnerClientId == player.ClientId);
            }
            else if (player.Type == PlayerType.Bot)
            {
                var possibleSources = FindObjectsByType<OnlineBotTeamInputSource>(FindObjectsSortMode.InstanceID);
                inputSource = possibleSources.First(s => s.GetComponent<NetworkObject>().OwnerClientId == player.ClientId);
            }
            else
            {
                Debug.LogError("Invalid player type when initializing teams");
            }
            team.Initialize(inputSource, player.TeamIndex, player.Name);

            if (player.Type == PlayerType.Bot)
            {
                botManagerFactory.CreateBotForTeam(team, settings.BotDifficulty);
            }
        }
        AllTeamsInitialized = true;
    }
}
