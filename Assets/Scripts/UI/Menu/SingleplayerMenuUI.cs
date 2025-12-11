using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SingleplayerMenuUI : MonoBehaviour
{
    [SerializeField] private MenuButtonUI _confirmButton;
    [SerializeField] private MenuButtonUI _cancelButton;
    [SerializeField] private MenuNumericDisplayUI _numBotsDisplay;
    [SerializeField] private MenuBotDifficultyToggleUI _botDifficultyToggle;
    [SerializeField] private MenuMapDisplayUI _mapDisplay;
    [SerializeField] private MenuCheckBoxUI _useTimerCheckbox;
    private LocalMenuInput _inputManager;

    private MenuUIManager _menuUIManager;

    private void Awake()
    {
        _menuUIManager = FindFirstObjectByType<MenuUIManager>();
        _inputManager = FindFirstObjectByType<LocalMenuInput>();
        _confirmButton.ButtonPressed += OnConfirmPressed;
        _cancelButton.ButtonPressed += OnCancelPressed;
        _numBotsDisplay.ValueChanged += OnNumberOfBotsChanged;
    }

    private void OnEnable()
    {
        _inputManager.MenuBackPerformed += _cancelButton.Press;
    }

    private void OnDisable()
    {
        _inputManager.MenuBackPerformed -= _cancelButton.Press;
    }

    private void Start()
    {
        _numBotsDisplay.Initialize(Constants.SingleplayerMinBots, Constants.SingleplayerMaxBots, Constants.SingleplayerMinBots);
        EventSystem.current.SetSelectedGameObject(_mapDisplay.gameObject);
    }

    public void OnConfirmPressed()
    {
        var settings = new GameplaySceneSettings()
        {
            SceneName = _mapDisplay.SelectedMap.SceneName,
            NumBots = _numBotsDisplay.Value,
            NumTeams = _numBotsDisplay.Value + 1,
            UseTimer = _useTimerCheckbox.Value,
            BotDifficulty = _botDifficultyToggle.Value,
            PlayerNames = new List<string>() { Constants.DefaultPlayerName }
        }; 
        _menuUIManager.HideAllPanels();
        SceneLoader.Instance.LoadGameplayScene(settings);
    }

    private void OnNumberOfBotsChanged(int numBots)
    {
        _mapDisplay.SetTeamCount(numBots+1);
    }

    public void OnCancelPressed()
    {
        _menuUIManager.SwitchToPreviousPanel();
    }

}
