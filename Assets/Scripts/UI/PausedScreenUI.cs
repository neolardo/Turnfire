using UnityEngine;
using UnityEngine.EventSystems;

public class PausedScreenUI : MonoBehaviour
{
    [SerializeField] private TextButtonUI _resumeButton;
    [SerializeField] private TextButtonUI _restartButton;
    [SerializeField] private TextButtonUI _exitButton;
    private GameplayInputManager _inputManager;


    private void Awake()
    {
        _resumeButton.ButtonPressed += OnResumeButtonPressed;
        _restartButton.ButtonPressed += OnRestartButtonPressed;
        _exitButton.ButtonPressed += OnExitButtonPressed;
        _inputManager = FindFirstObjectByType<GameplayInputManager>();
    }

    private void OnEnable()
    {
        _inputManager.PausedScreenConfirmPerformed += _resumeButton.PressIfHoveredOrSelected;
        _inputManager.PausedScreenConfirmPerformed += _restartButton.PressIfHoveredOrSelected;
        _inputManager.PausedScreenConfirmPerformed += _exitButton.PressIfHoveredOrSelected;
        EventSystem.current.SetSelectedGameObject(_resumeButton.gameObject);
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
