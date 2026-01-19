using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayUIManager : MonoBehaviour
{
    [SerializeField] private GameplaySettingsDefinition _gameplaySettings;
    [SerializeField] private GameObject _gameplayMenuUI;
    [SerializeField] private InventoryUI _inventoryUI;
    [SerializeField] private GameOverScreenUI _gameOverScreenUI;
    [SerializeField] private TeamHealthbarUIManager _teamHealthbarUIManager;
    [SerializeField] private GameplayTimerUI _gameplayTimer;
    [SerializeField] private CountdownTimerUI _countdownTimer;
    [SerializeField] private AimCircleUI _aimCircleUI;
    [SerializeField] private LoadingTextUI _loadingText;
    [SerializeField] private UISoundsDefinition _uiSounds;
    private LocalInputHandler _inputHandler;

    private bool _isDestroyed;
    private void Awake()
    {
        _inputHandler = FindFirstObjectByType<LocalInputHandler>();
        _inputHandler.ToggleInventoryPerformed += OnInventoryToggled;
        _inputHandler.AimStarted += _aimCircleUI.ShowCircles;
        _inputHandler.AimChanged += _aimCircleUI.UpdateCircles;
        _inputHandler.AimCancelled += _aimCircleUI.HideCircles;
        _inputHandler.ActionSkipped += OnActionSkipped;
        _inputHandler.ImpulseReleased += OnImpulseReleased;
        _gameplayTimer.gameObject.SetActive(false);
        _countdownTimer.gameObject.SetActive(true);
    }

    private void Start()
    {
        if (GameServices.IsInitialized)
        {
            OnGameServicesInitialized();
        }
        else
        {
            GameServices.Initialized += OnGameServicesInitialized;
        }
    }
            
    private void OnGameServicesInitialized()
    {
        GameServices.TurnStateManager.GameEnded += OnGameOver;
        GameServices.TurnStateManager.GameStarted += OnGameStarted;
        GameServices.GameStateManager.StateChanged += OnGameStateChanged;
        GameServices.CountdownTimer.TimerEnded += OnCountdownTimerEnded;
    }

    private void OnDestroy()
    {
        if(GameServices.TurnStateManager != null)
        {
            GameServices.TurnStateManager.GameEnded -= OnGameOver;
            GameServices.TurnStateManager.GameStarted -= OnGameStarted;
        }
        if (GameServices.GameStateManager != null)
        {
            GameServices.GameStateManager.StateChanged -= OnGameStateChanged;
        }
        if(GameServices.CountdownTimer != null)
        {
            GameServices.CountdownTimer.TimerEnded -= OnCountdownTimerEnded;
        }
        if(_inputHandler != null)
        {
            if(_aimCircleUI != null)
            {
                _inputHandler.AimStarted -= _aimCircleUI.ShowCircles;
                _inputHandler.AimChanged -= _aimCircleUI.UpdateCircles;
                _inputHandler.AimCancelled -= _aimCircleUI.HideCircles;
            }
            _inputHandler.ActionSkipped -= OnActionSkipped;
            _inputHandler.ImpulseReleased -= OnImpulseReleased;
        }
        _isDestroyed = true;
    }

    public void CreateTeamHealthbars(IEnumerable<Team> teams)
    {
        _teamHealthbarUIManager.CreateHealthBars(teams);
    }

    private void OnActionSkipped()
    {
        AudioManager.Instance.PlayUISound(_uiSounds.SkipAction);
    }

    private void OnImpulseReleased(Vector2 impulse)
    {
        _aimCircleUI.HideCircles();
    }

    private void OnGameplayMenuToggled(bool isGameplayMenuVisible)
    {
        if (_isDestroyed)
        {
            return;
        }
        _gameplayMenuUI.SetActive(isGameplayMenuVisible);
    }

    private void OnInventoryToggled()
    {
        if (_isDestroyed)
        {
            return;
        }
        _inventoryUI.gameObject.SetActive(!_inventoryUI.gameObject.activeSelf);
    }

    public void LoadCharacterData(Character character)
    {
        _inventoryUI.LoadCharacterData(character);
    }

    private void OnCountdownTimerEnded()
    {
        if(_isDestroyed)
        {
            return;
        }
        StartCoroutine(HideCountdownTimerAfterDelay());
    }

    private IEnumerator HideCountdownTimerAfterDelay()
    {
        yield return new WaitForSeconds(_gameplaySettings.DelaySecondsAfterCountdown);
        _countdownTimer.gameObject.SetActive(false);
    }

    public void ShowLoadingText()
    {
        _loadingText.gameObject.SetActive(true);
    }


    #region Game States

    public void OnGameStarted()
    {
        if (_isDestroyed)
        {
            return;
        }
        if (GameplaySceneSettingsStorage.Current.UseTimer)
        {
            _gameplayTimer.gameObject.SetActive(true);
        }
    }

    public void OnGameOver(Team winnerTeam)
    {
        if (_isDestroyed)
        {
            return;
        }
        GameServices.GameplayTimer.Pause();
        string gameOverText = string.Empty;
        if (winnerTeam == null)
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

    private void OnGameStateChanged(GameStateType gameState)
    {
        if (_isDestroyed)
        {
            return;
        }
        OnGameplayMenuToggled(gameState == GameStateType.Paused);
    }

    #endregion


}
