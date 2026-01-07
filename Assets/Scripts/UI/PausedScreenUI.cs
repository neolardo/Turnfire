using UnityEngine;
using UnityEngine.EventSystems;

public class PausedScreenUI : MonoBehaviour
{
    [SerializeField] private TextButtonUI _resumeButton;
    [SerializeField] private TextButtonUI _showControlsButton;
    [SerializeField] private TextButtonUI _restartButton;
    [SerializeField] private TextButtonUI _exitButton;
    [SerializeField] private ControlsPanelUI controlsPanel;
    private LocalGameplayInput _localInput;


    private void Awake()
    {
        _resumeButton.ButtonPressed += OnResumeButtonPressed;
        _showControlsButton.ButtonPressed += OnShowControlsButtonPressed;
        _restartButton.ButtonPressed += OnRestartButtonPressed;
        _exitButton.ButtonPressed += OnExitButtonPressed;
        _localInput = FindFirstObjectByType<LocalGameplayInput>();
    }

    private void OnEnable()
    {
        _localInput.PausedScreenConfirmPerformed += _resumeButton.PressIfHoveredOrSelected;
        _localInput.PausedScreenConfirmPerformed += _showControlsButton.PressIfHoveredOrSelected;
        _localInput.PausedScreenConfirmPerformed += _restartButton.PressIfHoveredOrSelected;
        _localInput.PausedScreenConfirmPerformed += _exitButton.PressIfHoveredOrSelected;
        EventSystem.current.SetSelectedGameObject(_resumeButton.gameObject);
    }

    private void OnDisable()
    {
        _localInput.PausedScreenConfirmPerformed -= _resumeButton.PressIfHoveredOrSelected;
        _localInput.PausedScreenConfirmPerformed -= _showControlsButton.PressIfHoveredOrSelected;
        _localInput.PausedScreenConfirmPerformed -= _restartButton.PressIfHoveredOrSelected;
        _localInput.PausedScreenConfirmPerformed -= _exitButton.PressIfHoveredOrSelected;
    }

    private void OnResumeButtonPressed()
    {
        _localInput.TogglePauseResumeGameplay();
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
