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

    private void Awake()
    {
        var inputHandler = FindFirstObjectByType<LocalInputHandler>();
        inputHandler.ToggleInventoryPerformed += OnInventoryToggled;
        _gameplayTimer.gameObject.SetActive(false);
        _countdownTimer.gameObject.SetActive(true);
    }
    private void Start()
    {
        GameServices.TurnStateManager.GameEnded += OnGameOver;
        GameServices.TurnStateManager.GameStarted += OnGameStarted;
        GameServices.GameStateManager.StateChanged += OnGameStateChanged;
        GameServices.CountdownTimer.TimerEnded += OnCountdownTimerEnded;
    }

    public void CreateTeamHealthbars(IEnumerable<Team> teams)
    {
        _teamHealthbarUIManager.CreateHealthBars(teams);
    }

    private void OnInventoryToggled()
    {
        _inventoryUI.gameObject.SetActive(!_inventoryUI.gameObject.activeSelf);
    }

    private void OnGameplayMenuToggled(bool isGameplayMenuVisible)
    {
        _gameplayMenuUI.SetActive(isGameplayMenuVisible);
    }

    public void LoadCharacterData(Character character)
    {
        _inventoryUI.LoadCharacterData(character);
    }

    private void OnCountdownTimerEnded()
    {
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

    #region Aim Circles

    public void ShowAimCircles(Vector2 initialPosition)
    {
        _aimCircleUI.ShowCircles(initialPosition);
    }

    public void UpdateAimCircles(Vector2 aimVector)
    {
        _aimCircleUI.UpdateCircles(aimVector);
    }

    public void HideAimCircles()
    {
        _aimCircleUI.HideCircles();
    } 

    #endregion

}
