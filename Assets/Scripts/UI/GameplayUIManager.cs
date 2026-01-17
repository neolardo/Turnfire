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
        // should I pass the teamsource here and visualize instead?
    }
            
    private void OnGameServicesInitialized()
    {
        GameServices.TurnStateManager.GameEnded += OnGameOver;
        GameServices.TurnStateManager.GameStarted += OnGameStarted;
        GameServices.GameStateManager.StateChanged += OnGameStateChanged;
        GameServices.CountdownTimer.TimerEnded += OnCountdownTimerEnded;
        Debug.Log("Game services initialized called from UI");
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
        _gameplayMenuUI.SetActive(isGameplayMenuVisible);
    }

    private void OnInventoryToggled()
    {
        _inventoryUI.gameObject.SetActive(!_inventoryUI.gameObject.activeSelf);
    }

    public void LoadCharacterData(Character character)
    {
        _inventoryUI.LoadCharacterData(character);
    }

    private void OnCountdownTimerEnded()
    {
        //TODO: why called after destroy?
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
        if (GameplaySceneSettingsStorage.Current.UseTimer)
        {
            _gameplayTimer.gameObject.SetActive(true);
        }
    }

    public void OnGameOver(Team winnerTeam)
    {
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
        OnGameplayMenuToggled(gameState == GameStateType.Paused);
    }

    #endregion


}
