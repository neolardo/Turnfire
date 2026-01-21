using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private MenuButtonUI _playButton;
    [SerializeField] private MenuButtonUI _settingsButton;
    [SerializeField] private MenuButtonUI _exitButton;

    private MenuUIManager _menuUIManager;

    private void Awake()
    {
        _playButton.ButtonPressed += OnPlayButtonPressed;
        _settingsButton.ButtonPressed += OnSettingsButtonPressed;
        _exitButton.ButtonPressed += OnExitButtonPressed;
        _menuUIManager = FindFirstObjectByType<MenuUIManager>();
    }

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(_playButton.gameObject);
    }

    private void OnPlayButtonPressed()
    {
        _menuUIManager.SwitchPanel(MenuPanelType.SingleplayerOrMultiplayerMenu);
    }

    private void OnSettingsButtonPressed()
    {
        _menuUIManager.SwitchPanel(MenuPanelType.SettingsMenu);
    }

    private void OnExitButtonPressed()
    {
        Application.Quit();
    }
    
}
