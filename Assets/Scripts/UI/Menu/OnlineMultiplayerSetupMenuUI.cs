using System.Collections;
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

    private LocalMenuInput _inputManager;
    private MenuUIManager _menuUIManager;

    private void Awake()
    {
        _menuUIManager = FindFirstObjectByType<MenuUIManager>();
        _inputManager = FindFirstObjectByType<LocalMenuInput>();
        _startButton.ButtonPressed += OnStartPressed;
        _cancelButton.ButtonPressed += OnCancelPressed;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientDisconnected;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton == null)
        {
            return;
        }

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
        RoomNetworkManager.LeaveRoom();
    }

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(_mapDisplay.gameObject);
    }


    #region Client Connnect

    private void OnClientConnected(ulong clientId)
    {
        RefreshJoinedPlayers();
    }

    private void OnClientDisconnected(ulong clientId)
    {
        RefreshJoinedPlayers();
        if (clientId == NetworkManager.ServerClientId)
        {
            LeaveRoomAndGoBack();
        }
    }

    private void RefreshJoinedPlayers()
    {
        int numJoinedPlayers = RoomNetworkSession.Instance.NumPlayers - 1;
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
        var clientIds = RoomNetworkSession.Instance.GetAllPlayerClientIds();
        int numPlayers = RoomNetworkSession.Instance.NumPlayers;
        var teamIds = Enumerable.Range(0, numPlayers).ToList();
        foreach(var clientId in clientIds) 
        {
            int teamId = teamIds[Random.Range(0, teamIds.Count)];
            teamIds.Remove(teamId);
            string playerName = RoomNetworkSession.Instance.GetPlayerName(clientId);
            players.Add(new Player(clientId, teamId, playerName, PlayerType.Human));
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
        LeaveRoomAndGoBack();
    }

    private void LeaveRoomAndGoBack()
    {
        RoomNetworkManager.LeaveRoom();
        _menuUIManager.SwitchToPreviousPanel();
    }
}
