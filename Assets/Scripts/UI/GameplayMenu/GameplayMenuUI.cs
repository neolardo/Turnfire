using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameplayMenuUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextButtonUI _resumeButton;
    [SerializeField] private TextButtonUI _restartButton;
    [SerializeField] private TextButtonUI _controlsButton;
    [SerializeField] private TextButtonUI _settingsButton;
    [SerializeField] private TextButtonUI _exitButton;
    [SerializeField] private ControlsPanelUI _controlsPanel;
    [SerializeField] private SettingsPanelUI _settingsPanel;
    private LocalInputHandler _inputHandler;


    private void Awake()
    {
        _resumeButton.ButtonPressed += OnResumeButtonPressed;
        _controlsButton.ButtonPressed += OnControlsButtonPressed;
        _settingsButton.ButtonPressed += OnSettingsButtonPressed;
        _exitButton.ButtonPressed += OnExitButtonPressed;
        _inputHandler = FindFirstObjectByType<LocalInputHandler>();
        if(GameplaySceneSettingsStorage.Current.IsOnlineGame)
        {
            _titleText.text = "MENU";
            if(NetworkManager.Singleton.IsServer)
            {
                _restartButton.ButtonPressed += OnRestartButtonPressed;
            }
        }
        else
        {
            _restartButton.ButtonPressed += OnRestartButtonPressed;
        }
    }

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(_resumeButton.gameObject);
    }

    private void OnResumeButtonPressed()
    {
        _inputHandler.ToggleGameplayMenu();
    }
    private void OnRestartButtonPressed()
    {
        GameServices.SceneLoader.ReloadScene();
        gameObject.SetActive(false);
    }

    private void OnControlsButtonPressed()
    {
        _controlsPanel.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
    private void OnSettingsButtonPressed()
    {
        _settingsPanel.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    private void OnExitButtonPressed()
    {
        GameServices.SceneLoader.LoadMenuScene();
        gameObject.SetActive(false);
    }

}
