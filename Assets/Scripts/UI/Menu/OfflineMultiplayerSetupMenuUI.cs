using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class OfflineMultiplayerSetupMenuUI : MonoBehaviour
{
    [SerializeField] private MenuButtonUI _confirmButton;
    [SerializeField] private MenuButtonUI _cancelButton;
    [SerializeField] private MenuNumericDisplayUI _numPlayersDisplay;
    [SerializeField] private MenuMapDisplayUI _mapDisplay;
    [SerializeField] private MenuCheckBoxUI _useTimerCheckbox;

    private LocalMenuUIInputSource _inputManager;
    private MenuUIManager _menuUIManager;
    private SceneLoaderFactory _sceneLoaderFactory;

    private void Awake()
    {
        _menuUIManager = FindFirstObjectByType<MenuUIManager>();
        _inputManager = FindFirstObjectByType<LocalMenuUIInputSource>();
        _sceneLoaderFactory = FindFirstObjectByType<SceneLoaderFactory>();
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
        _menuUIManager.HideAllPanelsAndShowLoadingText();
        _sceneLoaderFactory.TryCreateSceneLoader();
        OfflineSceneLoader.Instance.LoadGameplayScene(settings);
    }

    private GameplaySceneSettings CreateGameplaySceneSettings()
    {
        var players = new List<Player>();
        int numPlayers = _numPlayersDisplay.Value;
        var teamIds = Enumerable.Range(0, numPlayers).ToList();
        for (int playerId = 0; playerId < _numPlayersDisplay.Value; playerId++)
        {
            int teamId = teamIds[UnityEngine.Random.Range(0, teamIds.Count)];
            teamIds.Remove(teamId);
            players.Add(new Player(teamId, $"{Constants.DefaultPlayerName}{playerId + 1}", PlayerType.Human));
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
