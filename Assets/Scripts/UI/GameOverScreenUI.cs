using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameOverScreenUI : MonoBehaviour
{
    [SerializeField] private TextButtonUI _rematchButton;
    [SerializeField] private TextButtonUI _exitButton;
    [SerializeField] private TextMeshProUGUI _gameOverText;

    private void Awake()
    {
        _rematchButton.ButtonPressed += OnRematchButtonPressed;
        _exitButton.ButtonPressed += OnExitButtonPressed;
    }

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(_rematchButton.gameObject);
    }

    public void SetGameOverText(string text)
    {
        _gameOverText.text = text;
    }

    private void OnRematchButtonPressed()
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
