using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameplayMenuUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextButtonUI _resumeButton;
    [SerializeField] private TextButtonUI _showControlsButton;
    [SerializeField] private TextButtonUI _restartButton;
    [SerializeField] private TextButtonUI _exitButton;
    [SerializeField] private ControlsPanelUI controlsPanel;
    private LocalInputHandler _inputHandler;


    private void Awake()
    {
        _resumeButton.ButtonPressed += OnResumeButtonPressed;
        _showControlsButton.ButtonPressed += OnShowControlsButtonPressed;
        _restartButton.ButtonPressed += OnRestartButtonPressed;
        _exitButton.ButtonPressed += OnExitButtonPressed;
        _inputHandler = FindFirstObjectByType<LocalInputHandler>();
        if(GameplaySceneSettingsStorage.Current.IsOnlineGame)
        {
            _titleText.text = "";
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

    private void OnShowControlsButtonPressed()
    {
        controlsPanel.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    private void OnRestartButtonPressed()
    {
        GameServices.SceneLoader.ReloadScene();
        gameObject.SetActive(false);
    }

    private void OnExitButtonPressed()
    {
        GameServices.SceneLoader.LoadMenuScene();
        gameObject.SetActive(false);
    }

}
