using TMPro;
using UnityEngine;

public class PausedScreenUI : MonoBehaviour
{
    [SerializeField] private TextButtonUI _resumeButton;
    [SerializeField] private TextButtonUI _restartButton;
    [SerializeField] private TextButtonUI _exitButton;

    private InputManager _inputManager;

    private void Awake()
    {
        _inputManager = FindFirstObjectByType<InputManager>();
        _resumeButton.ButtonPressed += OnResumeButtonPressed;
        _restartButton.ButtonPressed += OnRestartButtonPressed;
        _exitButton.ButtonPressed += OnExitButtonPressed;
    }

    private void OnEnable()
    {
        _inputManager.PausedScreenConfirmPerformed += _resumeButton.PressIfHoveredOrSelected;
        _inputManager.PausedScreenConfirmPerformed += _restartButton.PressIfHoveredOrSelected;
        _inputManager.PausedScreenConfirmPerformed += _exitButton.PressIfHoveredOrSelected;
    }

    private void OnDisable()
    {
        _inputManager.PausedScreenConfirmPerformed -= _resumeButton.PressIfHoveredOrSelected;
        _inputManager.PausedScreenConfirmPerformed -= _restartButton.PressIfHoveredOrSelected;
        _inputManager.PausedScreenConfirmPerformed -= _exitButton.PressIfHoveredOrSelected;
    }

    private void OnResumeButtonPressed()
    {
        _inputManager.TogglePauseResumeGameplay();
    }

    private void OnRestartButtonPressed()
    {
        SceneLoader.Instance.ReloadScene();
        gameObject.SetActive(false);
    }

    private void OnExitButtonPressed()
    {
        SceneLoader.Instance.LoadMenuScene();
        gameObject.SetActive(false);
    }


}
