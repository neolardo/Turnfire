using UnityEngine;

public class MultiplayerMenuUI : MonoBehaviour
{
    [SerializeField] private MenuButtonUI _confirmButton;
    [SerializeField] private MenuButtonUI _cancelButton;
    [SerializeField] private MenuNumericDisplayUI _numPlayersDisplay;
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
        _numPlayersDisplay.ValueChanged += _mapDisplay.SetTeamCount;
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
        _numPlayersDisplay.Initialize(Constants.MultiplayerMinPlayers, Constants.MultiplayerMaxPlayers, Constants.MultiplayerMinPlayers);
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
            NumTeams = _numPlayersDisplay.Value,
            UseTimer = _useTimerCheckbox.Value
        };
        _menuUIManager.HideAllPanels();
        SceneLoader.Instance.LoadGameplayScene(settings);
    }

    public void OnCancelPressed()
    {
        _menuUIManager.SwitchToPreviousPanel();
    }

}
