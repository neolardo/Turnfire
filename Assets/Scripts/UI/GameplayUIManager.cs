using System;
using System.Collections.Generic;
using UnityEngine;

public class GameplayUIManager : MonoBehaviour
{
    [SerializeField] private GameObject _gameplayPausedScreen;
    [SerializeField] private InventoryUI _inventoryUI;
    [SerializeField] private GameOverScreenUI _gameOverScreenUI;
    [SerializeField] private TeamHealthbarUIManager _teamHealthbarUIManager;
    [SerializeField] private GameplayTimerUI _gameplayTimer;
    [SerializeField] private CountdownTimerUI _countdownTimer;
    private GameStateManager _gameStateManager;
    private bool _useTimer;

    public event Action GameplayTimerEnded;

    private void Awake()
    {
        var turnManager = FindFirstObjectByType<TurnManager>();
        turnManager.GameEnded += OnGameOver;
        turnManager.GameStarted += OnGameStarted;
        _gameStateManager = FindFirstObjectByType<GameStateManager>();
        _gameStateManager.StateChanged += OnGameStateChanged;
        var inputManager = FindFirstObjectByType<InputManager>();
        inputManager.ToggleInventoryPerformed += ToggleInventory;//TODO: refactor?
        _gameplayTimer.gameObject.SetActive(false);
        _gameplayTimer.TimerEnded += () => GameplayTimerEnded?.Invoke();
        _countdownTimer.gameObject.SetActive(true);
    }

    public void CreateTeamHealthbars(IEnumerable<Team> teams)
    {
        _teamHealthbarUIManager.CreateHealthBars(teams);
    }

    private void ToggleInventory()
    {
        _inventoryUI.gameObject.SetActive(!_inventoryUI.gameObject.activeSelf);
    }

    private void OnGameStateChanged(GameStateType gameState)
    {
        OnPause(gameState == GameStateType.Paused);
    }

    private void OnPause(bool pause)
    {
        _gameplayPausedScreen.SetActive(pause);
        if (_useTimer)
        {
            if (pause)
            {
                _gameplayTimer.StopTimer();
            }
            else
            {
                _gameplayTimer.ResumeTimer();
            }
        }
    }

    public void LoadCharacterData(Character character)
    {
        _inventoryUI.LoadCharacterData(character);
    }


    public void StartCountdown()
    {
        _countdownTimer.StartTimer();
    }

    public void StartGameplayTimer()
    {
        if(_useTimer && _gameStateManager.CurrentState == GameStateType.Playing)
        {
            _gameplayTimer.StartTimer();
        }
    }

    public void StopGameplayTimer()
    {
        if (_useTimer)
        {
            _gameplayTimer.StopTimer();
        }
    }

    public void OnGameStarted(GameplaySceneSettings gameplaySettings)
    {
        _useTimer = gameplaySettings.UseTimer;
        if(_useTimer)
        {
            _gameplayTimer.gameObject.SetActive(true);
        }
    }

    public void OnGameOver(Team winnerTeam)
    {
        StopGameplayTimer();
        string gameOverText = string.Empty; 
        if(winnerTeam == null)
        {
            gameOverText = "It's a tie!";
        }
        else
        {
            gameOverText = $"{winnerTeam.TeamName} wins!";
        }
        _gameOverScreenUI.SetGameOverText(gameOverText);
        _gameOverScreenUI.gameObject.SetActive(true);
    }

}
