using TMPro;
using UnityEngine;

public class GameOverScreenUI : MonoBehaviour
{
    [SerializeField] private TextButtonUI _rematchButton;
    [SerializeField] private TextButtonUI _exitButton;
    [SerializeField] private TextMeshProUGUI _gameOverText;

    private InputManager _inputManager;

    private void Awake()
    {
        _inputManager = FindFirstObjectByType<InputManager>();
        _rematchButton.ButtonPressed += OnRematchButtonPressed;
        _exitButton.ButtonPressed += OnExitButtonPressed;
    }

    private void OnEnable()
    {
        _inputManager.GameOverScreenConfirmPerformed += _rematchButton.PressIfHoveredOrSelected;
        _inputManager.GameOverScreenConfirmPerformed += _exitButton.PressIfHoveredOrSelected;
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
