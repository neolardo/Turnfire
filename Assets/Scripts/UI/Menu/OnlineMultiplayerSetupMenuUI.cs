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
    [SerializeField] private CheckBoxUI _useTimerCheckbox;
    [SerializeField] private TextMeshProUGUI _hostPlayerNameText;
    [SerializeField] private TMP_InputField _joinCodeInputField;
    [SerializeField] private TextMeshProUGUI _numPlayersJoinedText;

    private LocalMenuUIInputSource _inputManager;
    private MenuUIManager _menuUIManager;
    private SceneLoaderFactory _sceneLoaderFactory;

    private void Awake()
    {
        _menuUIManager = FindFirstObjectByType<MenuUIManager>();
        _inputManager = FindFirstObjectByType<LocalMenuUIInputSource>();
        _sceneLoaderFactory = FindFirstObjectByType<SceneLoaderFactory>();
        _startButton.ButtonPressed += OnStartPressed;
        _cancelButton.ButtonPressed += OnCancelPressed;
    }

    private void OnDestroy()
    {
        if (_startButton != null)
        {
            _startButton.ButtonPressed -= OnStartPressed;
        }
        if(_cancelButton != null)
        {
            _cancelButton.ButtonPressed -= OnCancelPressed;
        }
    }

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(_mapDisplay.gameObject);
        _inputManager.MenuBackPerformed += _cancelButton.Press;
        _inputManager.MenuIncrementValuePerformed += _useTimerCheckbox.OnDecrementOrIncrementValuePerformed;
        _inputManager.MenuDecrementValuePerformed += _useTimerCheckbox.OnDecrementOrIncrementValuePerformed;
        if (RoomNetworkSession.Instance != null)
        {
            RoomNetworkSession.Instance.RegisteredPlayersChanged += RefreshJoinedPlayers;
        }
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }
        StartCoroutine(OnOneFrameAfterOnEnable());
    }

    private IEnumerator OnOneFrameAfterOnEnable()
    {
        yield return null;
        RefreshJoinedPlayers();
    }

    private void OnDisable()
    {
        _inputManager.MenuBackPerformed -= _cancelButton.Press;
        _inputManager.MenuIncrementValuePerformed -= _useTimerCheckbox.OnDecrementOrIncrementValuePerformed;
        _inputManager.MenuDecrementValuePerformed -= _useTimerCheckbox.OnDecrementOrIncrementValuePerformed;
        if (RoomNetworkSession.Instance != null)
        {
            RoomNetworkSession.Instance.RegisteredPlayersChanged -= RefreshJoinedPlayers;
        }
        if(NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }


    #region Client Connnect

    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"{clientId} left the room");
        Debug.Log(NetworkManager.Singleton.DisconnectReason);
        if(NetworkManager.Singleton == null || !NetworkManager.Singleton.IsListening)
        {
            return;
        }
        if (clientId == NetworkManager.ServerClientId)
        {
            LeaveRoomAndGoBack();
        }
    }

    private void RefreshJoinedPlayers()
    {
        if(RoomNetworkSession.Instance == null)
        {
            return;
        }
        int numJoinedPlayers = RoomNetworkSession.Instance.NumPlayers;
        _numPlayersJoinedText.text = numJoinedPlayers.ToString();
        _startButton.SetIsInteractable(numJoinedPlayers > 1);
        _mapDisplay.SetTeamCount(numJoinedPlayers);
    }


    #endregion

    public void SetHostInfo(string hostPlayerName, string joinCode)
    {
        _hostPlayerNameText.text = hostPlayerName;
        _joinCodeInputField.text = joinCode;
    }

    public void OnStartPressed()
    {
        StartCoroutine(LoadSceneWhenAllClientsAreReady());
    }

    private IEnumerator LoadSceneWhenAllClientsAreReady()
    {
        _sceneLoaderFactory.TrySpawnNetworkSceneLoader();
        Debug.Log($"Waiting for all clients to spawn the {nameof(OnlineSceneLoader)}...");
        OnlineSceneLoader.Instance.RequestSpawnConfirmation();
        yield return new WaitUntil(() => OnlineSceneLoader.Instance.AllClientsHaveSpawned);
        Debug.Log($"All clients are ready, loading scene...");
        var settings = CreateGameplaySceneSettings();
        _menuUIManager.HideAllPanelsAndShowLoadingText();
        OnlineSceneLoader.Instance.LoadGameplayScene(settings);
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
            IsOnlineGame = true
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
