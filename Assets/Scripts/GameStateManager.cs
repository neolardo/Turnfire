using System;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public GameStateType CurrentState
    {
        get
        {
            return _state;
        }

        private set
        {
            if (_state != value)
            {
                _state = value;
                StateChanged?.Invoke(_state);
            }
        }
    }

    private GameStateType _state;

    public event Action <GameStateType> StateChanged;

    private void Awake()
    {
        _state = GameStateType.Playing;
        var inputManager = FindFirstObjectByType<InputManager>();
        var turnManager = FindFirstObjectByType<TurnManager>();
        inputManager.TogglePauseGameplayPerformed += OnTogglePauseResumeGameplay;
        turnManager.GameEnded += OnGameOver;
    }

    private void OnGameOver(Team winnerTeam)
    {
        CurrentState = GameStateType.GameOver;
    }

    private void OnTogglePauseResumeGameplay()
    {
        if(CurrentState == GameStateType.Playing)
        {
            CurrentState = GameStateType.Paused;
        }
        else
        {
            CurrentState = GameStateType.Playing;
        }
    }

}
