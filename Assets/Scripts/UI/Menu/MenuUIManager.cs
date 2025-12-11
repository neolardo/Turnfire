using System;
using UnityEngine;

public class MenuUIManager : MonoBehaviour
{
    [SerializeField] private MainMenuUI _mainMenu;
    [SerializeField] private SingleplayerMenuUI _singleplayerMenu;
    [SerializeField] private MultiplayerMenuUI _multiplayerMenu;

    private MenuPanelType _currentPanel;
    private MenuPanelType _previousPanel;


    private void Awake()
    {
        SwitchPanel(MenuPanelType.MainMenu);
    }

    public void SwitchPanel(MenuPanelType panel)
    {
        if(_currentPanel != panel)
        {
            _previousPanel = _currentPanel;
            _currentPanel = panel;

            HideAllPanels();

            var go = GetPanelGOFromType(panel);
            go.SetActive(true);
        }
    }

    public void SwitchToPreviousPanel()
    {
        SwitchPanel(_previousPanel);
    }

    public void HideAllPanels()
    {
        _mainMenu.gameObject.SetActive(false);
        _singleplayerMenu.gameObject.SetActive(false);
        _multiplayerMenu.gameObject.SetActive(false);
    }

    private GameObject GetPanelGOFromType(MenuPanelType panel)
    {
        switch(panel)
        {
            case MenuPanelType.MainMenu:
                return _mainMenu.gameObject;
            case MenuPanelType.SingleplayerMenu:
                return _singleplayerMenu.gameObject;
            case MenuPanelType.MultiplayerMenu:
                return _multiplayerMenu.gameObject;
            default:
                throw new Exception($"Invalid {nameof(MenuPanelType)} '{panel}'.");
        }
    }
}
