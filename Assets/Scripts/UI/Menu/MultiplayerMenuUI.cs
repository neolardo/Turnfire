using UnityEngine;

public class MultiplayerMenuUI : MonoBehaviour
{
    [SerializeField] private MenuButtonUI _confirmButton;
    [SerializeField] private MenuButtonUI _cancelButton;
    [SerializeField] private MenuNumericDisplayUI _numPlayersDisplay;
    [SerializeField] private MenuMapDisplayUI _mapDisplay;
    [SerializeField] private MenuCheckBoxUI _useTimerCheckbox;

    private MenuUIManager _menuUIManager;

    private void Awake()
    {
        _menuUIManager = FindFirstObjectByType<MenuUIManager>();
        _confirmButton.ButtonPressed += OnCofirmPressed;
        _cancelButton.ButtonPressed += OnCancelPressed;
    }

    private void Start()
    {
        _numPlayersDisplay.Initialize(Constants.MultiplayerMinPlayers, Constants.MultiplayerMaxPlayers, Constants.MultiplayerMinPlayers);
    }

    public void OnCofirmPressed()
    {
        var settings = new GameplaySceneSettings()
        {
            SceneName = Constants.MapSceneNamePrefix + _mapDisplay.MapIndex,
            NumTeams = _numPlayersDisplay.Value,
            UseTimer = _useTimerCheckbox.Value
        };
        _menuUIManager.HideAllPanels();
        SceneLoader.Instance.LoadScene(settings);
    }

    public void OnCancelPressed()
    {
        _menuUIManager.SwitchToPreviousPanel();
    }

}
