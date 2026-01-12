using System;
using UnityEngine;

public class OfflineGameStateManager : MonoBehaviour, IGameStateManager
{
    [SerializeField] private GameplaySettingsDefinition _gameplaySettings;
    private GameStateManagerLogic _logic;
    public GameStateType CurrentState => _logic.CurrentState;

    public event Action<GameStateType> StateChanged;

    private void Awake()
    {
        _logic = new GameStateManagerLogic(_gameplaySettings);
        _logic.StateChanged += InvokeStateChanged;
    }

    public void StartGame()
    {
        StartCoroutine(_logic.StartGameAfterCountdownCoroutine());
    }

    private void InvokeStateChanged(GameStateType newState)
    {
        StateChanged?.Invoke(newState);
    }

    public void TogglePauseResumeGameplay()
    {
        _logic.TogglePauseResumeGameplay();
    }

}
