using System;
using System.Collections;
using UnityEngine;

public class GameStateManagerLogic
{
    private GameplaySettingsDefinition _gameplaySettings;
    private bool _countdownEnded;

    private GameStateType _state;
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

    public event Action<GameStateType> StateChanged;

    public GameStateManagerLogic(GameplaySettingsDefinition settings)
    {
        _gameplaySettings = settings;
    }


    private void OnCountdownEnded()
    {
        _countdownEnded = true;
    }

    public IEnumerator StartGameAfterCountdownCoroutine()
    {
        Debug.Log("waiting for turnstate manager to be initialized");
        yield return new WaitUntil(() => GameServices.TurnStateManager.IsInitialized);
        GameServices.CountdownTimer.TimerEnded += OnCountdownEnded;
        GameServices.TurnStateManager.GameEnded += OnGameOver;
        Debug.Log("waiting for countdown timer to be initialized");
        yield return new WaitUntil(() => GameServices.CountdownTimer.IsInitialized);
        Debug.Log("countdown timer started");
        GameServices.CountdownTimer.Restart();
        Debug.Log("waiting for countdown timer to end");
        yield return new WaitUntil(() => _countdownEnded);
        Debug.Log("countdown timer ended");
        yield return new WaitForSeconds(_gameplaySettings.DelaySecondsAfterCountdown);
        Debug.Log("starting game");
        StartGame();
    }

    private void StartGame()
    {
        CurrentState = GameStateType.Playing;
        GameServices.GameplayTimer.Restart();
        GameServices.GameplayTimer.Pause();
        GameServices.TurnStateManager.StartFirstTurn();
    }

    private void OnGameOver(Team winnerTeam)
    {
        CurrentState = GameStateType.GameOver;
    }

    public void TogglePauseResumeGameplay()
    {
        if (CurrentState == GameStateType.Playing)
        {
            CurrentState = GameStateType.Paused;
        }
        else
        {
            CurrentState = GameStateType.Playing;
        }
    }
}
