using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnlineMultiplayerSetupMenuUI : MonoBehaviour
{
    [SerializeField] private MenuButtonUI _startButton;
    [SerializeField] private MenuButtonUI _cancelButton;
    [SerializeField] private MenuMapDisplayUI _mapDisplay;
    [SerializeField] private MenuCheckBoxUI _useTimerCheckbox;
    [SerializeField] private TextMeshProUGUI _hostPlayerNameText;
    [SerializeField] private TextMeshProUGUI _joinCodeText;
    [SerializeField] private TextMeshProUGUI _numPlayersJoinedText;

    private List<ulong> _playerClientIdList;

    private LocalMenuInput _inputManager;
    private MenuUIManager _menuUIManager;

    private void Awake()
    {
        _playerClientIdList = new List<ulong>();
        _menuUIManager = FindFirstObjectByType<MenuUIManager>();
        _inputManager = FindFirstObjectByType<LocalMenuInput>();
        _startButton.ButtonPressed += OnStartPressed;
        _cancelButton.ButtonPressed += OnCancelPressed;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientDisconnected;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton == null) return;

        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
    }

    private void OnEnable()
    {
        _inputManager.MenuBackPerformed += _cancelButton.Press;
        RefreshJoinedPlayers();
    }

    private void OnDisable()
    {
        _inputManager.MenuBackPerformed -= _cancelButton.Press;
        NetworkRoomManager.LeaveRoom();
    }

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(_mapDisplay.gameObject);
    }


    #region Client Connnect

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"Client connected: {clientId}");
        _playerClientIdList.Add(clientId);
        RefreshJoinedPlayers();
    }

    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"Client disconnected: {clientId}");
        _playerClientIdList.Remove(clientId);
        RefreshJoinedPlayers();
        if (clientId == NetworkManager.ServerClientId)
        {
            LeaveRoom();
        }
    }

    private void RefreshJoinedPlayers()
    {
        int numJoinedPlayers = _playerClientIdList.Count - 1;
        _numPlayersJoinedText.text = numJoinedPlayers.ToString();
        _startButton.SetIsInteractable(numJoinedPlayers > 0);
        _mapDisplay.SetTeamCount(numJoinedPlayers + 1);
    }


    #endregion

    public void SetHostInfo(string hostPlayerName, string joinCode)
    {
        _hostPlayerNameText.text = hostPlayerName;
        _joinCodeText.text = joinCode;
    }

    public void OnStartPressed()
    {
        var settings = CreateGameplaySceneSettings();
        _menuUIManager.HideAllPanels();
        SceneLoader.Instance.LoadGameplayScene(settings);
    }

    private GameplaySceneSettings CreateGameplaySceneSettings()
    {
        var players = new List<Player>();
        int numPlayers = _playerClientIdList.Count;
        var teamIds = Enumerable.Range(0, numPlayers).ToList();
        for (int playerId = 0; playerId < numPlayers; playerId++)
        {
            int teamId = teamIds[Random.Range(0, teamIds.Count)];
            teamIds.Remove(teamId);
            players.Add(new Player(_playerClientIdList[playerId], teamId, $"{Constants.DefaultPlayerName}{playerId + 1}", PlayerType.Human));
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
        LeaveRoom();
    }

    private void LeaveRoom()
    {
        NetworkRoomManager.LeaveRoom();
        _menuUIManager.SwitchToPreviousPanel();
    }
}
