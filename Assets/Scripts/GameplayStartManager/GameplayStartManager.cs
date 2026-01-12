using System.Collections;
using UnityEngine;

public class GameplayStartManager : MonoBehaviour
{
    [SerializeField] private TeamComposerBootstrap _teamComposerBootstrap;
    private void Start()
    {
        StartCoroutine(StartGameplay());
    }

    private IEnumerator StartGameplay()
    {
        yield return new WaitUntil(() => GameServices.IsInitialized);
        yield return new WaitUntil(() => _teamComposerBootstrap.AllTeamsInitialized);
        GameServices.TurnStateManager.Initialize(_teamComposerBootstrap.GetTeams());
        GameServices.GameStateManager.StartGame();
    }
}
