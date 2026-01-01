using System;
using System.Collections.Generic;
using UnityEngine;

public class MenuUIManager : MonoBehaviour
{
    [SerializeField] private MainMenuUI _mainMenu;
    [SerializeField] private SingleplayerMenuUI _singleplayerMenu;
    [SerializeField] private OnlineOrOfflineMultiplayerMenuUI _onlineOrOfflineMultiplayerMenu;
    [SerializeField] private HostOrJoinMultiplayerMenuUI _hostOrJoinMultiplayerMenu;
    [SerializeField] private OfflineMultiplayerSetupMenuUI _offlineMultiplayerSetupMenu;
    [SerializeField] private OnlineMultiplayerSetupMenuUI _onlineMultiplayerSetupMenu;
    [SerializeField] private JoinRoomMultiplayerMenuUI _joinMultiplayerMenu;
    [SerializeField] private CreateRoomMultiplayerMenuUI _hostMultiplayerMenu;

    private Stack<MenuPanelType> _previousPanels;
    private MenuPanelType _currentPanel;


    private void Awake()
    {
        _previousPanels = new Stack<MenuPanelType>();
        SwitchPanel(MenuPanelType.MainMenu);
    }

    public void SwitchPanel(MenuPanelType panel, bool previous = false)
    {
        if(_currentPanel != panel)
        {
            if(_currentPanel != MenuPanelType.None && !previous)
            {
                _previousPanels.Push(_currentPanel);
            }
            _currentPanel = panel;

            HideAllPanels();

            var go = GetPanelGOFromType(panel);
            go.SetActive(true);
        }
    }

    public void SwitchToPreviousPanel()
    {
        SwitchPanel(_previousPanels.Pop(), true);
    }

    public void HideAllPanels()
    {
        _mainMenu.gameObject.SetActive(false);
        _singleplayerMenu.gameObject.SetActive(false);
        _onlineOrOfflineMultiplayerMenu.gameObject.SetActive(false);
        _hostOrJoinMultiplayerMenu.gameObject.SetActive(false);
        _hostMultiplayerMenu.gameObject.SetActive(false);
        _joinMultiplayerMenu.gameObject.SetActive(false);
        _offlineMultiplayerSetupMenu.gameObject.SetActive(false);
        _onlineMultiplayerSetupMenu.gameObject.SetActive(false);
    }

    private GameObject GetPanelGOFromType(MenuPanelType panel)
    {
        switch(panel)
        {
            case MenuPanelType.MainMenu:
                return _mainMenu.gameObject;
            case MenuPanelType.SingleplayerMenu:
                return _singleplayerMenu.gameObject;
            case MenuPanelType.HostOrJoinMultiplayerMenu:
                return _hostOrJoinMultiplayerMenu.gameObject;
            case MenuPanelType.OnlineOrOfflineMultiplayerMenu:
                return _onlineOrOfflineMultiplayerMenu.gameObject;
            case MenuPanelType.CreateRoomMultiplayerMenu:
                return _hostMultiplayerMenu.gameObject;
            case MenuPanelType.JoinRoomMultiplayerMenu:
                return _joinMultiplayerMenu.gameObject;
            case MenuPanelType.OnlineMultiplayerSetupMenu:
                return _onlineMultiplayerSetupMenu.gameObject;
            case MenuPanelType.OfflineMultiplayerSetupMenu:
                return _offlineMultiplayerSetupMenu.gameObject;
            default:
                throw new Exception($"Invalid {nameof(MenuPanelType)} '{panel}'.");
        }
    }
}
