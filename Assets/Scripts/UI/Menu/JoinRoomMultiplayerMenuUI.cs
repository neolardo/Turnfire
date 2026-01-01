using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoinRoomMultiplayerMenuUI : MonoBehaviour
{
    [SerializeField] private MenuButtonUI _joinButton;
    [SerializeField] private MenuButtonUI _backButton;
    [SerializeField] private TextMeshProUGUI _joinCodeText;
    [SerializeField] private TextMeshProUGUI _playerNameText;
    [SerializeField] private TextMeshProUGUI _responseText;
    private LocalMenuInput _inputManager;

    private bool _isJoined;

    private MenuUIManager _menuUIManager;

    private void Awake()
    {
        _menuUIManager = FindFirstObjectByType<MenuUIManager>();
        _inputManager = FindFirstObjectByType<LocalMenuInput>();
        _joinButton.ButtonPressed += OnJoinPressed;
        _backButton.ButtonPressed += OnCancelPressed;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    private void OnEnable()
    {
        InitializeUI();
        _playerNameText.text = Constants.DefaultPlayerName;
        _inputManager.MenuBackPerformed += _backButton.Press;
    }

    private void InitializeUI()
    {
        _responseText.text = "";
        _joinCodeText.text = "";
        _backButton.SetText("cancel");
        _isJoined = false;
        _joinButton.SetIsInteractable(false);
    }

    private void OnDisable()
    {
        _inputManager.MenuBackPerformed -= _backButton.Press;
        NetworkRoomManager.LeaveRoom();
    }

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(_joinButton.gameObject);
    }

    private void OnDestroy()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (clientId == NetworkManager.ServerClientId)
        {
            Debug.Log("Host left the room.");
            LeaveRoom();
        }
    }

    public void OnJoinPressed()
    {
        _ = TryJoinRoom(_joinCodeText.text, _playerNameText.text);
    }

    private async Task TryJoinRoom(string joinCode, string playerName)
    {
        _joinButton.SetIsInteractable(false);
        _responseText.text = "Joining room...\nPlease wait.";
        var result = await NetworkRoomManager.TryJoinRoomAsync(joinCode, playerName);
        if (result == NetworkRoomResult.Ok)
        {
            _responseText.text = "Join successful.\nWaiting for the host to start the game.";
            _backButton.SetText("leave");
            _isJoined = true;
        }
        else if (result == NetworkRoomResult.NetworkError)
        {
            _responseText.text = "Cannot join room.\nPlease check your internet connection and try again.";
            _joinButton.SetIsInteractable(true);
        }
        else if (result == NetworkRoomResult.PlayerNameInvalid)
        {
            if (string.IsNullOrWhiteSpace(playerName))
            {
                _responseText.text = "Cannot join room.\nPlease enter a player name.";
            }
            else
            {
                _responseText.text = "Cannot join room.\nThis player name already exists in the room.";
            }
            _joinButton.SetIsInteractable(true);
        }
        else if (result == NetworkRoomResult.JoinCodeInvalid)
        {
            _responseText.text = "Cannot join room.\nJoin code is invalid.";
            _joinButton.SetIsInteractable(true);
        }
    }

    public void OnCancelPressed()
    {
        if (_isJoined)
        {
            LeaveRoom();
        }
        else
        { 
            _menuUIManager.SwitchToPreviousPanel();
        }
    }

    private void LeaveRoom()
    {
        NetworkRoomManager.LeaveRoom();
        InitializeUI();
    }
}
