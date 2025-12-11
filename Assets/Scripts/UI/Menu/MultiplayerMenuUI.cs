using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
        _inputManager.MenuBackPerformed += _cancelButton.Press;
    }

    private void OnDisable()
    {
        _inputManager.MenuBackPerformed -= _cancelButton.Press;
    }

    private void Start()
    {
        _numPlayersDisplay.Initialize(Constants.MultiplayerMinPlayers, Constants.MultiplayerMaxPlayers, Constants.MultiplayerMinPlayers);
        EventSystem.current.SetSelectedGameObject(_mapDisplay.gameObject);
    }

    public void OnConfirmPressed()
    {
        var playerNames = new List<string>(); //TODO: get names from menu textbox?
        for (int i = 0; i < _numPlayersDisplay.Value; i++)
        {
            playerNames.Add(Constants.DefaultPlayerName + i + 1);
        }

        var settings = new GameplaySceneSettings()
        {
            SceneName = _mapDisplay.SelectedMap.SceneName,
            NumTeams = _numPlayersDisplay.Value,
            UseTimer = _useTimerCheckbox.Value,
            PlayerNames = playerNames
        };
        _menuUIManager.HideAllPanels();
        SceneLoader.Instance.LoadGameplayScene(settings);
    }

    public void OnCancelPressed()
    {
        _menuUIManager.SwitchToPreviousPanel();
    }

}
