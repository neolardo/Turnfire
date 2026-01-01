using UnityEngine;
using UnityEngine.EventSystems;

public class HostOrJoinMultiplayerMenuUI : MonoBehaviour
{
    [SerializeField] private MenuButtonUI _hostButton;
    [SerializeField] private MenuButtonUI _joinButton;
    [SerializeField] private MenuButtonUI _backButton;

    private MenuUIManager _menuUIManager;

    private void Awake()
    {
        _hostButton.ButtonPressed += OnHostButtonPressed;
        _joinButton.ButtonPressed += OnJoinButtonPressed;
        _backButton.ButtonPressed += OnBackButtonPressed;
        _menuUIManager = FindFirstObjectByType<MenuUIManager>();
    }

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(_hostButton.gameObject);
    }

    private void OnHostButtonPressed()
    {
        _menuUIManager.SwitchPanel(MenuPanelType.CreateRoomMultiplayerMenu);
    }

    private void OnJoinButtonPressed()
    {
        _menuUIManager.SwitchPanel(MenuPanelType.JoinRoomMultiplayerMenu);
    }

    private void OnBackButtonPressed()
    {
        _menuUIManager.SwitchToPreviousPanel();
    }

}
