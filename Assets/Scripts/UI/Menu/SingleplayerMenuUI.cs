using UnityEngine;

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
        //TODO: wire in controller input
    }

    private void OnEnable()
    {
        _inputManager.MenuConfirmPerformed += _confirmButton.Press;
        _inputManager.MenuBackPerformed += _cancelButton.Press;
        _inputManager.MenuToggleCheckboxPerformed += OnMenuToggleCheckboxPerformed;
    }

    private void OnDisable()
    {
        _inputManager.MenuConfirmPerformed -= _confirmButton.Press;
        _inputManager.MenuBackPerformed -= _cancelButton.Press;
        _inputManager.MenuToggleCheckboxPerformed -= OnMenuToggleCheckboxPerformed;
    }

    private void Start()
    {
        _numBotsDisplay.Initialize(Constants.SingleplayerMinBots, Constants.SingleplayerMaxBots, Constants.SingleplayerMinBots);
    }

    private void OnMenuToggleCheckboxPerformed()
    {
        _useTimerCheckbox.ToggleValue(true);
    }

    public void OnConfirmPressed()
    {
        var settings = new GameplaySceneSettings()
        {
            SceneName = _mapDisplay.SelectedMap.SceneName,
            NumBots = _numBotsDisplay.Value,
            NumTeams = _numBotsDisplay.Value+1,
            UseTimer = _useTimerCheckbox.Value,
            BotDifficulty = _botDifficultyToggle.Value
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
