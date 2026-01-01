using UnityEngine;
using UnityEngine.EventSystems;

public class OnlineOrOfflineMultiplayerMenuUI : MonoBehaviour
{
    [SerializeField] private MenuButtonUI _onlineButton;
    [SerializeField] private MenuButtonUI _offlineButton;
    [SerializeField] private MenuButtonUI _backButton;

    private MenuUIManager _menuUIManager;

    private void Awake()
    {
        _onlineButton.ButtonPressed += OnOnlineButtonPressed;
        _offlineButton.ButtonPressed += OnOfflineButtonPressed;
        _backButton.ButtonPressed += OnBackButtonPressed;
        _menuUIManager = FindFirstObjectByType<MenuUIManager>();
    }

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(_onlineButton.gameObject);
    }

    private void OnOnlineButtonPressed()
    {
        _menuUIManager.SwitchPanel(MenuPanelType.HostOrJoinMultiplayerMenu);
    }

    private void OnOfflineButtonPressed()
    {
        _menuUIManager.SwitchPanel(MenuPanelType.OfflineMultiplayerSetupMenu);
    }

    private void OnBackButtonPressed()
    {
        _menuUIManager.SwitchToPreviousPanel();
    }

}
