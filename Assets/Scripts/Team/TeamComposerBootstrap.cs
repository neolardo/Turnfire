using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class TeamComposerBootstrap : MonoBehaviour
{
    [SerializeField] private List<Team> _possibleTeams;
    private Team[] _teams;

    public bool AllTeamsInitialized => _composer == null ? false : _composer.AllTeamsInitialized;

    private ITeamComposer _composer;

    private void Start()
    {
        ActvatePlayableTeams();
        StartComposeTeams();
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

    private void StartComposeTeams()
    {
        bool isOnlineGame = GameplaySceneSettingsStorage.Current.IsOnlineGame;
        if (isOnlineGame)
        {
            StartCoroutine(CreateOnlineComposerAndComposeTeams(transform));
        }
        else
        {
            StartCoroutine(CreateOfflineComposerAndComposeTeams(transform));
        }
    }

    private IEnumerator CreateOfflineComposerAndComposeTeams(Transform parent)
    {
        _composer = this.AddComponent<OfflineTeamComposer>();
        _composer.ComposeTeams(_teams);
        yield break;
    }

    private IEnumerator CreateOnlineComposerAndComposeTeams(Transform parent)
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            yield break;
        }
        _composer = this.AddComponent<OnlineTeamComposer>();
        var netObj = this.GetComponent<NetworkObject>();
        netObj.Spawn();
        yield return new WaitUntil(() => netObj.IsSpawned);
        _composer.ComposeTeams(_teams);
    }

    public IEnumerable<Team> GetTeams()
    {
        return _teams;
    }

}
