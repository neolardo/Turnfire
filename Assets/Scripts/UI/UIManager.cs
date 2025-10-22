using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _gameOverText;
    [SerializeField] private GameObject _gameOverScreen;
    [SerializeField] private GameObject _gameplayPausedScreen;
    [SerializeField] private GameObject _inventoryPanel;
    [SerializeField] private InventoryUI _inventoryUI;
    [SerializeField] private TeamHealthbarUIManager _teamHealthbarUIManager;
    [SerializeField] private GameplayTimerUI _gameplayTimer;
    [SerializeField] private CountdownTimerUI _countdownTimer;
    private bool _useTimer;

    public event Action GameplayTimerEnded;

    private void Awake()
    {
        var turnManager = FindFirstObjectByType<TurnManager>();
        turnManager.GameEnded += OnGameOver;
        turnManager.GameStarted += OnGameStarted;
        var gameStateManager = FindFirstObjectByType<GameStateManager>();
        gameStateManager.StateChanged += OnGameStateChanged;
        var inputManager = FindFirstObjectByType<InputManager>();
        inputManager.ToggleInventoryPerformed += ToggleInventory;//TODO: refactor?
        _gameplayTimer.gameObject.SetActive(false);
        _gameplayTimer.TimerEnded += () => GameplayTimerEnded?.Invoke();
    }

    public void CreateTeamHealthbars(IEnumerable<Team> teams)
    {
        _teamHealthbarUIManager.CreateHealthBars(teams);
    }

    private void ToggleInventory()
    {
        _inventoryPanel.SetActive(!_inventoryPanel.activeSelf);
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
        if(_useTimer)
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
        if(winnerTeam == null)
        {
            _gameOverText.text = "It's a tie!";
        }
        else
        {
            _gameOverText.text = $"{winnerTeam.TeamName} wins!";
        }
        _gameOverScreen.SetActive(true);
    }

}
