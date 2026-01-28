using System;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class CreateRoomMultiplayerMenuUI : MonoBehaviour
{
    [SerializeField] private MenuButtonUI _hostButton;
    [SerializeField] private MenuButtonUI _cancelButton;
    [SerializeField] private TextMeshProUGUI _hostInfoText;
    [SerializeField] private TMP_InputField _hostNameInputField;
    [SerializeField] private OnlineMultiplayerSetupMenuUI _onlineMultiplayerSetup;
    [SerializeField] private RoomNetworkSession _roomSessionPrefab;

    private LocalMenuUIInputSource _inputManager;
    private MenuUIManager _menuUIManager;

    private void Awake()
    {
        _menuUIManager = FindFirstObjectByType<MenuUIManager>();
        _inputManager = FindFirstObjectByType<LocalMenuUIInputSource>();
        _hostButton.ButtonPressed += OnHostPressed;
        _cancelButton.ButtonPressed += OnCancelPressed;
    }

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(_hostNameInputField.gameObject);
        _inputManager.MenuBackPerformed += _cancelButton.Press;
        _hostInfoText.text = "";
        _hostNameInputField.text = "";
        _hostButton.SetIsInteractable(true);
        if(NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }
    }

    private void OnDisable()
    {
        _inputManager.MenuBackPerformed -= _cancelButton.Press;
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (clientId != NetworkManager.Singleton.LocalClientId)
            return;

        var reason = NetworkManager.Singleton.DisconnectReason;
        if (reason == Constants.InvalidNameReasonValue)
        {
            _hostInfoText.text = "Failed to create room.\nPlayer name was invalid.";
        }
        _hostButton.SetIsInteractable(true);
    }

    public void OnHostPressed()
    {
        _ = TryHostRoom(_hostNameInputField.text);
    }

    private void TrySpawnRoomSession()
    {
        if (RoomNetworkSession.Instance == null)
        {
            try
            {
                var roomSession = Instantiate(_roomSessionPrefab);
                roomSession.GetComponent<NetworkObject>().Spawn();
            }
            catch(Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }

    private async Task TryHostRoom(string playerName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(playerName))
            {
                playerName = Constants.DefaultPlayerName;
            }
            _hostButton.SetIsInteractable(false);
            _hostInfoText.text = "Creating room...\nPlease wait.";
            var (result, joinCode) = await RoomNetworkManager.TryHostRoomAsync(playerName);
            if (result == RoomNetworkConnectionResult.Ok)
            {
                DontDestroyOnLoad(NetworkManager.Singleton.gameObject);
                TrySpawnRoomSession();
                RoomNetworkSession.Instance.TryRegisterPlayer(NetworkManager.Singleton.LocalClientId, playerName);
                _hostInfoText.text = "";
                _onlineMultiplayerSetup.SetHostInfo(playerName, joinCode);
                _menuUIManager.SwitchPanel(MenuPanelType.OnlineMultiplayerSetupMenu);
            }
            else if (result == RoomNetworkConnectionResult.NetworkError)
            {
                _hostInfoText.text = "Failed to create room.\nCheck your connection.";
                _hostButton.SetIsInteractable(true);
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }
  

    public void OnCancelPressed()
    {
        RoomNetworkManager.LeaveRoom();
        _menuUIManager.SwitchToPreviousPanel();
    }

}
