using System.Collections;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(NetworkReadyGate))]
public class GameplayStartManager : MonoBehaviour
{
    [SerializeField] private TeamManager _teamComposerBootstrap;
    private NetworkReadyGate _readyGate;
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
        yield return WaitUntilReadyGateIsSpawned();

        yield return new WaitUntil(() => GameServices.IsInitialized);
        Debug.Log("Game services initialized");
        yield return _readyGate.WaitUntilEveryClientIsReadyCoroutine();

        _teamComposerBootstrap.InitializeTeams();
        yield return new WaitUntil(() => _teamComposerBootstrap.AllTeamsInitialized);
        Debug.Log("All teams initialized");
        yield return _readyGate.WaitUntilEveryClientIsReadyCoroutine();

        _teamComposerBootstrap.CreateAndSelectInitialItems();
        Debug.Log("All initial items created and selected");
        yield return _readyGate.WaitUntilEveryClientIsReadyCoroutine();

        GameServices.TurnStateManager.Initialize(_teamComposerBootstrap.GetTeams());
        Debug.Log("Turn state manager initialized");
        yield return _readyGate.WaitUntilEveryClientIsReadyCoroutine();


        GameServices.GameStateManager.StartGame();
    }

    private IEnumerator WaitUntilReadyGateIsSpawned()
    {
        _readyGate = GetComponent<NetworkReadyGate>();
        var gateNetObj = _readyGate.GetComponent<NetworkObject>();
        if (NetworkManager.Singleton.IsServer && !gateNetObj.IsSpawned)
        {
            gateNetObj.Spawn();
        }
        yield return new WaitUntil(() => gateNetObj.IsSpawned);
    }


}
