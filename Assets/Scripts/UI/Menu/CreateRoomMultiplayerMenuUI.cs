using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using WebSocketSharp;

public class CreateRoomMultiplayerMenuUI : MonoBehaviour
{
    [SerializeField] private MenuButtonUI _hostButton;
    [SerializeField] private MenuButtonUI _cancelButton;
    [SerializeField] private TextMeshProUGUI _hostInfoText;
    [SerializeField] private TextMeshProUGUI _hostPlayerNameText;
    [SerializeField] private OnlineMultiplayerSetupMenuUI _onlineMultiplayerSetup;

    private LocalMenuInput _inputManager;
    private MenuUIManager _menuUIManager;

    private void Awake()
    {
        _menuUIManager = FindFirstObjectByType<MenuUIManager>();
        _inputManager = FindFirstObjectByType<LocalMenuInput>();
        _hostButton.ButtonPressed += OnHostPressed;
        _cancelButton.ButtonPressed += OnCancelPressed;
    }

    private void OnEnable()
    {
        _inputManager.MenuBackPerformed += _cancelButton.Press;
        _hostInfoText.text = "";
        _hostPlayerNameText.text = "";
        _hostButton.SetIsInteractable(false);
    }

    private void OnDisable()
    {
        _inputManager.MenuBackPerformed -= _cancelButton.Press;
        NetworkRoomManager.LeaveRoom();
    }

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(_hostButton.gameObject);
    }

    public void OnHostPressed()
    {
        _ = TryHostRoom(_hostPlayerNameText.text);
    }

    private async Task TryHostRoom(string playerName)
    {
        _hostButton.SetIsInteractable(false);
        _hostInfoText.text = "Creating room...\nPlease wait.";
        var (result, joinCode) = await NetworkRoomManager.TryHostRoomAsync(playerName);
        if(result == NetworkRoomResult.Ok)
        {
            _hostInfoText.text = "";
            _onlineMultiplayerSetup.SetHostInfo(playerName, joinCode);
            _menuUIManager.SwitchPanel(MenuPanelType.OnlineMultiplayerSetupMenu);
        }
        else if(result == NetworkRoomResult.NetworkError)
        {
            _hostInfoText.text = "Cannot create room.\nPlease check your internet connection and try again.";
            _hostButton.SetIsInteractable(true);
        }
        else if(result == NetworkRoomResult.PlayerNameInvalid)
        {
            if(playerName.IsNullOrEmpty())
            {
                _hostInfoText.text = "Cannot create room.\nPlease enter a player name.";
            }
            else
            {
                _hostInfoText.text = "Cannot create room.\nPlease enter a valid player name.";
            }
            _hostButton.SetIsInteractable(true);
        }
    }
  

    public void OnCancelPressed()
    {
        _menuUIManager.SwitchToPreviousPanel();
    }

}
