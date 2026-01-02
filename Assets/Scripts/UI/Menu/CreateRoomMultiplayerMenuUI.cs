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

    private LocalMenuInput _inputManager;
    private MenuUIManager _menuUIManager;

    private void Awake()
    {
        _menuUIManager = FindFirstObjectByType<MenuUIManager>();
        _inputManager = FindFirstObjectByType<LocalMenuInput>();
        _hostButton.ButtonPressed += OnHostPressed;
        _cancelButton.ButtonPressed += OnCancelPressed;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    private void OnEnable()
    {
        _inputManager.MenuBackPerformed += _cancelButton.Press;
        _hostInfoText.text = "";
        _hostNameInputField.text = "";
        _hostButton.SetIsInteractable(true);
    }

    private void OnDisable()
    {
        _inputManager.MenuBackPerformed -= _cancelButton.Press;
    }

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(_hostNameInputField.gameObject);
    }

    private void OnDestroy()
    {
        if(NetworkManager.Singleton == null)
        {
            return;
        }
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
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
        if(string.IsNullOrWhiteSpace(playerName))
        {
            playerName = Constants.DefaultPlayerName;
        }
        _hostButton.SetIsInteractable(false);
        _hostInfoText.text = "Creating room...\nPlease wait.";
        var (result, joinCode) = await RoomNetworkManager.TryHostRoomAsync(playerName);
        if(result == RoomNetworkConnectionResult.Ok)
        {
            DontDestroyOnLoad(NetworkManager.Singleton.gameObject);
            TrySpawnRoomSession();
            RoomNetworkSession.Instance.TryRegisterPlayer(NetworkManager.Singleton.LocalClientId, playerName);
            _hostInfoText.text = "";
            _onlineMultiplayerSetup.SetHostInfo(playerName, joinCode);
            _menuUIManager.SwitchPanel(MenuPanelType.OnlineMultiplayerSetupMenu);
        }
        else if(result == RoomNetworkConnectionResult.NetworkError)
        {
            _hostInfoText.text = "Failed to create room.\nPlease check your internet connection and try again.";
            _hostButton.SetIsInteractable(true);
        }
    }
  

    public void OnCancelPressed()
    {
        RoomNetworkManager.LeaveRoom();
        _menuUIManager.SwitchToPreviousPanel();
    }

}
