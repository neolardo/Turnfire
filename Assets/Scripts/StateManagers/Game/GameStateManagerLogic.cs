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
        GameServices.CountdownTimer.TimerEnded += OnCountdownEnded;
        GameServices.TurnStateManager.GameEnded += OnGameOver;
    }


    private void OnCountdownEnded()
    {
        _countdownEnded = true;
    }

    public IEnumerator StartGameAfterCountdownCoroutine()
    {
        yield return new WaitUntil(() => GameServices.CountdownTimer.IsInitialized);
        GameServices.CountdownTimer.Restart();
        yield return new WaitUntil(() => _countdownEnded);
        yield return new WaitForSeconds(_gameplaySettings.DelaySecondsAfterCountdown);
        yield return new WaitUntil(() => GameServices.TurnStateManager.IsInitialized);
        StartGame();
    }

    private void StartGame()
    {
        CurrentState = GameStateType.Playing;
        GameServices.GameplayTimer.Restart();
        GameServices.GameplayTimer.Pause();
        GameServices.TurnStateManager.StartGame();
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
