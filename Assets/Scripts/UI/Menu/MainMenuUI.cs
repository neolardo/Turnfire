using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private MenuButtonUI _singleplayerButton;
    [SerializeField] private MenuButtonUI _multiplayerButton;
    [SerializeField] private MenuButtonUI _exitButton;

    private MenuUIManager _menuUIManager;

    private void Awake()
    {
        _singleplayerButton.ButtonPressed += OnSingleplayerButtonPressed;
        _multiplayerButton.ButtonPressed += OnMultiplayerButtonPressed;
        _exitButton.ButtonPressed += OnExitButtonPressed;
        _menuUIManager = FindFirstObjectByType<MenuUIManager>();
    }

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(_multiplayerButton.gameObject);
    }

    private void OnSingleplayerButtonPressed()
    {
    }

    private void OnMultiplayerButtonPressed()
    {
        _menuUIManager.SwitchPanel(MenuPanelType.MultiplayerMenu);
    }

    private void OnExitButtonPressed()
    {
        Application.Quit();
    }
    
}
