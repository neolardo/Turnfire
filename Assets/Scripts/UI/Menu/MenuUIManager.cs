using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUIManager : MonoBehaviour
{
    [SerializeField] private MainMenuUI _mainMenu;
    [SerializeField] private SingleplayerOrMultiplayerMenuUI _singleplayerOrMultiplayerMenu;
    [SerializeField] private MenuSettingsUI _menuSettingsUI;
    [SerializeField] private SingleplayerMenuUI _singleplayerMenu;
    [SerializeField] private OnlineOrOfflineMultiplayerMenuUI _onlineOrOfflineMultiplayerMenu;
    [SerializeField] private HostOrJoinMultiplayerMenuUI _hostOrJoinMultiplayerMenu;
    [SerializeField] private OfflineMultiplayerSetupMenuUI _offlineMultiplayerSetupMenu;
    [SerializeField] private OnlineMultiplayerSetupMenuUI _onlineMultiplayerSetupMenu;
    [SerializeField] private JoinRoomMultiplayerMenuUI _joinRoomMultiplayerMenu;
    [SerializeField] private CreateRoomMultiplayerMenuUI _createRoomMultiplayerMenu;
    [SerializeField] private LoadingTextUI _loadingText;
    [SerializeField] private CanvasGroup _canvasGroup;

    private Stack<MenuPanelType> _previousPanels;
    private MenuPanelType _currentPanel;


    private void Awake()
    {
        _previousPanels = new Stack<MenuPanelType>();
        _canvasGroup.alpha = 0;
        ShowAllPanels();
    }

    private void OnEnable()
    {
        StartCoroutine(ShowOneFrameAfterOnEnable());
    }

    private IEnumerator ShowOneFrameAfterOnEnable()
    {
        yield return null;
        HideAllPanels();
        SwitchPanel(MenuPanelType.MainMenu);
        _canvasGroup.alpha = 1;
    }

    public void SwitchPanel(MenuPanelType panel, bool previous = false)
    {
        if(_currentPanel != panel)
        {
            if(_currentPanel != MenuPanelType.None && !previous)
            {
                _previousPanels.Push(_currentPanel);
            }
            if (panel != MenuPanelType.None) // show current
            {
                var nextGo = GetPanelGOFromType(panel);
                nextGo.SetActive(true);
            }
            if(_currentPanel != MenuPanelType.None) // hide last
            {
                var lastGo = GetPanelGOFromType(_currentPanel);
                lastGo.SetActive(false);
            }
            _currentPanel = panel;
        }
    }

    public void SwitchToPreviousPanel()
    {
        SwitchPanel(_previousPanels.Pop(), true);
    }

    private void ShowAllPanels()
    {
        _mainMenu.gameObject.SetActive(true);
        _singleplayerOrMultiplayerMenu.gameObject.SetActive(true);
        _menuSettingsUI.gameObject.SetActive(true);
        _singleplayerMenu.gameObject.SetActive(true);
        _onlineOrOfflineMultiplayerMenu.gameObject.SetActive(true);
        _hostOrJoinMultiplayerMenu.gameObject.SetActive(true);
        _createRoomMultiplayerMenu.gameObject.SetActive(true);
        _joinRoomMultiplayerMenu.gameObject.SetActive(true);
        _offlineMultiplayerSetupMenu.gameObject.SetActive(true);
        _onlineMultiplayerSetupMenu.gameObject.SetActive(true);
    }

    public void HideAllPanels()
    {
        _mainMenu.gameObject.SetActive(false);
        _singleplayerOrMultiplayerMenu.gameObject.SetActive(false);
        _menuSettingsUI.gameObject.SetActive(false);
        _singleplayerMenu.gameObject.SetActive(false);
        _onlineOrOfflineMultiplayerMenu.gameObject.SetActive(false);
        _hostOrJoinMultiplayerMenu.gameObject.SetActive(false);
        _createRoomMultiplayerMenu.gameObject.SetActive(false);
        _joinRoomMultiplayerMenu.gameObject.SetActive(false);
        _offlineMultiplayerSetupMenu.gameObject.SetActive(false);
        _onlineMultiplayerSetupMenu.gameObject.SetActive(false);
    }

    private void ShowLoadingText()
    {
        _loadingText.gameObject.SetActive(true);
    }

    public void HideAllPanelsAndShowLoadingText()
    {
        HideAllPanels();
        ShowLoadingText();
    }

    private GameObject GetPanelGOFromType(MenuPanelType panel)
    {
        switch(panel)
        {
            case MenuPanelType.MainMenu:
                return _mainMenu.gameObject;
            case MenuPanelType.SettingsMenu:
                return _menuSettingsUI.gameObject;
            case MenuPanelType.SingleplayerOrMultiplayerMenu:
                return _singleplayerOrMultiplayerMenu.gameObject;
            case MenuPanelType.SingleplayerMenu:
                return _singleplayerMenu.gameObject;
            case MenuPanelType.HostOrJoinMultiplayerMenu:
                return _hostOrJoinMultiplayerMenu.gameObject;
            case MenuPanelType.OnlineOrOfflineMultiplayerMenu:
                return _onlineOrOfflineMultiplayerMenu.gameObject;
            case MenuPanelType.CreateRoomMultiplayerMenu:
                return _createRoomMultiplayerMenu.gameObject;
            case MenuPanelType.JoinRoomMultiplayerMenu:
                return _joinRoomMultiplayerMenu.gameObject;
            case MenuPanelType.OnlineMultiplayerSetupMenu:
                return _onlineMultiplayerSetupMenu.gameObject;
            case MenuPanelType.OfflineMultiplayerSetupMenu:
                return _offlineMultiplayerSetupMenu.gameObject;
            default:
                throw new Exception($"Invalid {nameof(MenuPanelType)} '{panel}'.");
        }
    }
}
