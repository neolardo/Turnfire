using System.Collections;
using UnityEngine;

public class GameplayStartManager : MonoBehaviour
{
    [SerializeField] private TeamManager _teamComposerBootstrap;
    private void Start()
    {
        StartCoroutine(StartGameplay());
    }

    private IEnumerator StartGameplay()
    {
        yield return new WaitUntil(() => GameServices.IsInitialized);
        Debug.Log("Game services initialized");
        yield return new WaitUntil(() => _teamComposerBootstrap.AllTeamsInitialized);
        Debug.Log("All teams initialized");
        GameServices.TurnStateManager.Initialize(_teamComposerBootstrap.GetTeams());
        GameServices.GameStateManager.StartGame();
    }
}
