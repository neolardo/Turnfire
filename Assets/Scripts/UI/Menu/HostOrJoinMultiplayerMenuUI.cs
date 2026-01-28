using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class HostOrJoinMultiplayerMenuUI : MonoBehaviour
{
    [SerializeField] private MenuButtonUI _hostButton;
    [SerializeField] private MenuButtonUI _joinButton;
    [SerializeField] private MenuButtonUI _backButton;

    private MenuUIManager _menuUIManager;
    private LocalMenuUIInputSource _inputManager;

    private void Awake()
    {
        _hostButton.ButtonPressed += OnHostButtonPressed;
        _joinButton.ButtonPressed += OnJoinButtonPressed;
        _backButton.ButtonPressed += OnBackButtonPressed;
        _menuUIManager = FindFirstObjectByType<MenuUIManager>();
        _inputManager = FindFirstObjectByType<LocalMenuUIInputSource>();
    }
    private void OnEnable()
    {
        StartCoroutine(SelectDefaultButtonNextFrame());
        _inputManager.MenuBackPerformed += _backButton.Press;
    }

    private IEnumerator SelectDefaultButtonNextFrame()
    {
        yield return null;
        EventSystem.current.SetSelectedGameObject(_hostButton.gameObject);
    }

    private void OnDisable()
    {
        _inputManager.MenuBackPerformed -= _backButton.Press;
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
