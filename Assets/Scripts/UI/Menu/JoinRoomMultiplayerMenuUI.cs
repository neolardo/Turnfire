using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoinRoomMultiplayerMenuUI : MonoBehaviour
{
    [SerializeField] private MenuButtonUI _joinButton;
    [SerializeField] private MenuButtonUI _backButton;
    [SerializeField] private TMP_InputField _joinCodeInputField;
    [SerializeField] private TMP_InputField _playerNameInputField;
    [SerializeField] private TextMeshProUGUI _responseText;
    private LocalMenuInput _inputManager;

    private bool _isJoinInitiated;

    private MenuUIManager _menuUIManager;

    private void Awake()
    {
        _menuUIManager = FindFirstObjectByType<MenuUIManager>();
        _inputManager = FindFirstObjectByType<LocalMenuInput>();
        _joinButton.ButtonPressed += OnJoinPressed;
        _backButton.ButtonPressed += OnCancelPressed;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    private void OnEnable()
    {
        InitializeUI();
        _playerNameInputField.text = "";
        _inputManager.MenuBackPerformed += _backButton.Press;
    }

    private void InitializeUI()
    {
        _responseText.text = "";
        _joinCodeInputField.text = "";
        _backButton.SetText("cancel");
        _isJoinInitiated = false;
        _joinButton.SetIsInteractable(false);
    }

    private void OnDisable()
    {
        _inputManager.MenuBackPerformed -= _backButton.Press;
        RoomNetworkManager.LeaveRoom();
    }

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(_joinCodeInputField.gameObject);
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

    public void OnInputFieldValueChanged(string value)
    {
        var joinCode = _joinCodeInputField.text;
        var clientName = _playerNameInputField.text;
        bool areFieldsValid = 
            !string.IsNullOrWhiteSpace(joinCode) && joinCode.Length > 0 &&
            !string.IsNullOrWhiteSpace(clientName) && clientName.Length > 0;
        _joinButton.SetIsInteractable(areFieldsValid);
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (clientId == NetworkManager.ServerClientId)
        {
            Debug.Log("Host left the room.");
            LeaveRoom();
            return;
        }

        if (clientId != NetworkManager.Singleton.LocalClientId)
            return;

        var reason = NetworkManager.Singleton.DisconnectReason;
        if (reason == Constants.InvalidNameReasonValue)
        {
            _responseText.text = "Failed to join room.\nPlayer name was invalid.";
        }
        _isJoinInitiated = false;
        _backButton.SetText("cancel");
    }

    private void OnClientConnected(ulong clientId)
    {
        if(clientId == NetworkManager.Singleton.LocalClientId)
        {
            _responseText.text = "Join successful.\nWaiting for the host to start the game.";
        }
    }

    public void OnJoinPressed()
    {
        _ = TryJoinRoom(_joinCodeInputField.text, _playerNameInputField.text);
    }

    private async Task TryJoinRoom(string joinCode, string playerName)
    {
        _isJoinInitiated = true;
        _backButton.SetText("leave");
        _joinButton.SetIsInteractable(false);
        _responseText.text = "Joining room...\nPlease wait.";
        if (string.IsNullOrWhiteSpace(playerName))
        {
            playerName = Constants.DefaultPlayerName;
        }
        var result = await RoomNetworkManager.TryJoinRoomAsync(joinCode, playerName);
        if (result == RoomNetworkConnectionResult.Ok)
        {
            DontDestroyOnLoad(NetworkManager.Singleton.gameObject);
            // wait for connected callback
        }
        else if (result == RoomNetworkConnectionResult.NetworkError)
        {
            _responseText.text = "Failed to join room.\nPlease check your internet connection and try again.";
            _joinButton.SetIsInteractable(true);
        }
        else if (result == RoomNetworkConnectionResult.JoinCodeInvalid)
        {
            _responseText.text = "Failed to join room.\nJoin code was invalid.";
            _joinButton.SetIsInteractable(true);
        }
    }

    public void OnCancelPressed()
    {
        if (_isJoinInitiated)
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
        RoomNetworkManager.LeaveRoom();
        InitializeUI();
    }
}
