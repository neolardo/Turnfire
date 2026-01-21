using UnityEngine;
using UnityEngine.EventSystems;

public class SingleplayerOrMultiplayerMenuUI : MonoBehaviour
{
    [SerializeField] private MenuButtonUI _singleplayerButton;
    [SerializeField] private MenuButtonUI _multiplayerButton;
    [SerializeField] private MenuButtonUI _backButton;

    private MenuUIManager _menuUIManager;

    private void Awake()
    {
        _singleplayerButton.ButtonPressed += OnSingleplayerButtonPressed;
        _multiplayerButton.ButtonPressed += OnMultiplayerButtonPressed;
        _backButton.ButtonPressed += OnBackButtonPressed;
        _menuUIManager = FindFirstObjectByType<MenuUIManager>();
    }

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(_singleplayerButton.gameObject);
    }

    private void OnSingleplayerButtonPressed()
    {
        _menuUIManager.SwitchPanel(MenuPanelType.SingleplayerMenu);
    }

    private void OnMultiplayerButtonPressed()
    {
        _menuUIManager.SwitchPanel(MenuPanelType.OnlineOrOfflineMultiplayerMenu);
    }
    private void OnBackButtonPressed()
    {
        _menuUIManager.SwitchToPreviousPanel();
    }
}
