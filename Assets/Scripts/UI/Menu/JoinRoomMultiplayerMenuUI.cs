using System;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class JoinRoomMultiplayerMenuUI : MonoBehaviour
{
    [SerializeField] private MenuButtonUI _joinButton;
    [SerializeField] private MenuButtonUI _backButton;
    [SerializeField] private TMP_InputField _joinCodeInputField;
    [SerializeField] private TMP_InputField _playerNameInputField;
    [SerializeField] private TextMeshProUGUI _responseText;
    private LocalMenuUIInputSource _inputManager;

    private bool _isJoinInitiated;
    private bool _isLeaveIntentional;

    private MenuUIManager _menuUIManager;

    private void Awake()
    {
        _menuUIManager = FindFirstObjectByType<MenuUIManager>();
        _inputManager = FindFirstObjectByType<LocalMenuUIInputSource>();
        _joinButton.ButtonPressed += OnJoinPressed;
        _backButton.ButtonPressed += OnCancelPressed;
    }

    private void OnDestroy()
    {
        if(_joinButton != null)
        {
            _joinButton.ButtonPressed -= OnJoinPressed;
        }
        if(_backButton != null)
        {
            _backButton.ButtonPressed -= OnCancelPressed;
        }
    }

    private void OnEnable()
    {
        InitializeUI();
        EventSystem.current.SetSelectedGameObject(_joinCodeInputField.gameObject);
        _playerNameInputField.text = "";
        _joinCodeInputField.text = "";
        _inputManager.MenuBackPerformed += _backButton.Press;
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }
    }

    private void InitializeUI()
    {
        _responseText.text = "";
        _backButton.SetText("cancel");
        _isJoinInitiated = false;
        _playerNameInputField.interactable = true;
        _joinCodeInputField.interactable = true;
        RefreshJoinButtonIsInteractable();
    }

    private void OnDisable()
    {
        _inputManager.MenuBackPerformed -= _backButton.Press;
        if(NetworkManager.Singleton != null)
        {
            if (NetworkManager.Singleton.SceneManager != null)
            {
                NetworkManager.Singleton.SceneManager.OnLoad -= OnSceneLoadStarted;
            }

            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }

    private void OnSceneLoadStarted(ulong clientId, string sceneName, LoadSceneMode loadSceneMode, AsyncOperation asyncOperation)
    {
        _menuUIManager.HideAllPanelsAndShowLoadingText();
    }

    public void OnInputFieldValueChanged(string _)
    {
        RefreshJoinButtonIsInteractable();
    }

    private void RefreshJoinButtonIsInteractable()
    {
        var joinCode = _joinCodeInputField.text;
        var clientName = _playerNameInputField.text;
        bool areFieldsValid =
            !string.IsNullOrWhiteSpace(joinCode) && joinCode.Length > 0 &&
            !string.IsNullOrWhiteSpace(clientName) && clientName.Length > 0;
        _joinButton.SetIsInteractable(areFieldsValid);
    }

    private void OnClientConnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            NetworkManager.Singleton.SceneManager.OnLoad += OnSceneLoadStarted;
            _responseText.text = "Join successful.\nWaiting for the host to start.";
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"{clientId} left the room");

        if (clientId != NetworkManager.Singleton.LocalClientId || NetworkManager.Singleton.SceneManager == null || !NetworkManager.Singleton.IsListening)
        {
            return;
        }
        NetworkManager.Singleton.SceneManager.OnLoad -= OnSceneLoadStarted;
        var reason = NetworkManager.Singleton.DisconnectReason;
        if (reason == Constants.InvalidNameReasonValue)
        {
            _responseText.text = "Failed to join room.\nPlayer name is taken.";
        }
        else if (reason == Constants.RoomIsFullReasonValue)
        {
            _responseText.text = "Failed to join room.\nRoom is full.";
        }
        else if(_isLeaveIntentional)
        {
            _responseText.text = "";
        }
        else
        {
            Debug.Log($"Host (most likely) left.");
            Debug.Log(reason);
            _responseText.text = "Host left.\nTry joining another room.";
        }
        _isLeaveIntentional = false;
        _isJoinInitiated = false;
        EnableInputs();
        _backButton.SetText("cancel");
    }


    public void OnJoinPressed()
    {
        _ = TryJoinRoom(_joinCodeInputField.text, _playerNameInputField.text);
    }

    private async Task TryJoinRoom(string joinCode, string playerName)
    {
        try
        {
            _isJoinInitiated = true;
            _backButton.SetText("leave");
            DisableInputs();
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
            else
            {
                if (result == RoomNetworkConnectionResult.NetworkError)
                {
                    _responseText.text = "Failed to join room.\nCheck your internet connection.";
                }
                else if (result == RoomNetworkConnectionResult.JoinCodeInvalid)
                {
                    _responseText.text = "Failed to join room.\nJoin code was invalid.";
                }
                _isJoinInitiated = false;
                EnableInputs();
                _backButton.SetText("cancel");
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    private void EnableInputs()
    {
        RefreshJoinButtonIsInteractable();
        _playerNameInputField.interactable = true;
        _joinCodeInputField.interactable = true;
    }

    private void DisableInputs()
    {
        _joinButton.SetIsInteractable(false);
        _playerNameInputField.interactable = false;
        _joinCodeInputField.interactable = false;
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
        _isLeaveIntentional = true;
        RoomNetworkManager.LeaveRoom();
        InitializeUI();
    }
}
