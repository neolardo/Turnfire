using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameOverScreenUI : MonoBehaviour
{
    [SerializeField] private TextButtonUI _rematchButton;
    [SerializeField] private TextButtonUI _exitButton;
    [SerializeField] private TextMeshProUGUI _gameOverText;
    private LocalGameplayInput _inputManager;

    private void Awake()
    {
        _inputManager = FindFirstObjectByType<LocalGameplayInput>();
        _rematchButton.ButtonPressed += OnRematchButtonPressed;
        _exitButton.ButtonPressed += OnExitButtonPressed;
    }

    private void OnEnable()
    {
        _inputManager.GameOverScreenConfirmPerformed += _rematchButton.PressIfHoveredOrSelected;
        _inputManager.GameOverScreenConfirmPerformed += _exitButton.PressIfHoveredOrSelected;
        EventSystem.current.SetSelectedGameObject(_rematchButton.gameObject);
    }

    private void OnDisable()
    {
        _inputManager.GameOverScreenConfirmPerformed -= _rematchButton.PressIfHoveredOrSelected;
        _inputManager.GameOverScreenConfirmPerformed -= _exitButton.PressIfHoveredOrSelected;
    }

    public void SetGameOverText(string text)
    {
        _gameOverText.text = text;
    }

    private void OnRematchButtonPressed()
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
