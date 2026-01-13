using System.Collections;
using UnityEngine;

public class GameplayStartManager : MonoBehaviour
{
    [SerializeField] private TeamManager _teamComposerBootstrap;
    private void Start()
    {
        if(GameplaySceneSettingsStorage.Current.IsOnlineGame)
        {
            StartCoroutine(StartOnlineGameplay());
        }
        else
        {
            StartCoroutine(StartOfflineGameplay());
        }
    }

    private IEnumerator StartOfflineGameplay()
    {
        yield return new WaitUntil(() => GameServices.IsInitialized);
        Debug.Log("Game services initialized");
        _teamComposerBootstrap.InitializeTeams();
        yield return new WaitUntil(() => _teamComposerBootstrap.AllTeamsInitialized);
        Debug.Log("All teams initialized");
        GameServices.TurnStateManager.Initialize(_teamComposerBootstrap.GetTeams());
        Debug.Log("Turn state manager initialized");
        GameServices.GameStateManager.StartGame();
    }

    private IEnumerator StartOnlineGameplay()
    {
        var readyGate = FindAnyObjectByType<NetworkReadyGate>();

        yield return new WaitUntil(() => GameServices.IsInitialized);
        Debug.Log("Game services initialized");
        readyGate.MarkReady();
        yield return new WaitUntil(() => readyGate.AllClientsReady);
        readyGate.AcknowledgeReady();

        _teamComposerBootstrap.InitializeTeams();
        yield return new WaitUntil(() => _teamComposerBootstrap.AllTeamsInitialized);
        Debug.Log("All teams initialized");
        readyGate.MarkReady();
        yield return new WaitUntil(() => readyGate.AllClientsReady);
        readyGate.AcknowledgeReady();

        GameServices.TurnStateManager.Initialize(_teamComposerBootstrap.GetTeams());
        Debug.Log("Turn state manager initialized");
        readyGate.MarkReady();
        yield return new WaitUntil(() => readyGate.AllClientsReady);
        readyGate.AcknowledgeReady();

        GameServices.GameStateManager.StartGame();
    }
}
