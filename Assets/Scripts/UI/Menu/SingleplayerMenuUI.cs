using System.Collections.Generic;
using System.Linq;
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
        var settings = CreateGameplaySceneSettings();
        _menuUIManager.HideAllPanels();
        SceneLoader.Instance.LoadGameplayScene(settings);
    }

    private GameplaySceneSettings CreateGameplaySceneSettings()
    {
        int numPlayers = _numBotsDisplay.Value+1;
        var teamIds = Enumerable.Range(0, numPlayers).ToList();
        int teamId = teamIds[Random.Range(0, teamIds.Count)];
        teamIds.Remove(teamId);
        var players = new List<Player>
        {
            new Player(teamId, Constants.DefaultPlayerName, PlayerType.Human)
        };
        for (int botId = 0; botId < _numBotsDisplay.Value; botId++)
        {
            teamId = teamIds[Random.Range(0, teamIds.Count)];
            teamIds.Remove(teamId);
            players.Add(new Player(teamId, $"{Constants.DefaultBotName}{botId+1}", PlayerType.Bot));
        }

        return new GameplaySceneSettings()
        {
            Map = _mapDisplay.SelectedMap,
            UseTimer = _useTimerCheckbox.Value,
            BotDifficulty = _botDifficultyToggle.Value,
            Players = players,
            IsOnlineGame = false
        };
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
