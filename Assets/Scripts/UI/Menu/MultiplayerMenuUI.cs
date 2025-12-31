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
        var settings = CreateGameplaySceneSettings();
        _menuUIManager.HideAllPanels();
        SceneLoader.Instance.LoadGameplayScene(settings);
    }

    private GameplaySceneSettings CreateGameplaySceneSettings()
    {
        //TODO: names from textbox? needed?
        var players = new List<Player>();
        for (int teamId = 0; teamId < _numPlayersDisplay.Value; teamId++)
        {
            //TODO: create team setup here instead of at the start
            players.Add(new Player(teamId,$"{Constants.DefaultPlayerName}{teamId + 1}", PlayerType.Human));
        }

        return new GameplaySceneSettings()
        {
            Map = _mapDisplay.SelectedMap,
            UseTimer = _useTimerCheckbox.Value,
            Players = players,
            IsOnlineGame = false
        };
    }

    public void OnCancelPressed()
    {
        _menuUIManager.SwitchToPreviousPanel();
    }

}
