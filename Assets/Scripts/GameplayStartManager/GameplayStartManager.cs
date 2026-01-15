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

        _teamComposerBootstrap.CreateAndSelectInitialItems();
        Debug.Log("All initial items created and selected");

        GameServices.TurnStateManager.Initialize(_teamComposerBootstrap.GetTeams());
        Debug.Log("Turn state manager initialized");

        GameServices.GameStateManager.StartGame();
    }

    private IEnumerator StartOnlineGameplay()
    {
        var readyGate = FindAnyObjectByType<NetworkReadyGate>();

        yield return new WaitUntil(() => GameServices.IsInitialized);
        Debug.Log("Game services initialized");
        yield return WaitUntilEveryClientIsReady(readyGate);

        _teamComposerBootstrap.InitializeTeams();
        yield return new WaitUntil(() => _teamComposerBootstrap.AllTeamsInitialized);
        Debug.Log("All teams initialized");
        yield return WaitUntilEveryClientIsReady(readyGate);

        _teamComposerBootstrap.CreateAndSelectInitialItems();
        Debug.Log("All initial items created and selected");
        yield return WaitUntilEveryClientIsReady(readyGate);

        GameServices.TurnStateManager.Initialize(_teamComposerBootstrap.GetTeams());
        Debug.Log("Turn state manager initialized");
        yield return WaitUntilEveryClientIsReady(readyGate);


        GameServices.GameStateManager.StartGame();
    }

    private IEnumerator WaitUntilEveryClientIsReady(NetworkReadyGate readyGate)
    {
        readyGate.MarkReady();
        yield return new WaitUntil(() => readyGate.AllClientsReady);
        readyGate.AcknowledgeReady();
        yield return new WaitUntil(() => readyGate.AllClientsAcknowledgedReady);
    }
}
