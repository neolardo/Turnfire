using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] MenuButtonUI _singleplayerButton;
    [SerializeField] MenuButtonUI _multiplayerButton;
    [SerializeField] MenuButtonUI _exitButton;

    void Start()
    {
        _singleplayerButton.ButtonPressed += OnSingleplayerButtonPressed;
        _multiplayerButton.ButtonPressed += OnMultiplayerButtonPressed;
        _exitButton.ButtonPressed += OnExitButtonPressed;
    }

    private void OnSingleplayerButtonPressed()
    {
    }

    private void OnMultiplayerButtonPressed()
    {
        
    }

    private void OnExitButtonPressed()
    {
        Application.Quit();
    }
    
}
