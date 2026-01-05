using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlsPanelUI : MonoBehaviour
{
    private LocalGameplayInput _localInput;

    [SerializeField] private TextMeshProUGUI _aimText;
    [SerializeField] private TextMeshProUGUI _jumpAndShootText;
    [SerializeField] private TextMeshProUGUI _skipText;
    [SerializeField] private TextMeshProUGUI _openInventoryText;
    [SerializeField] private TextMeshProUGUI _showMapText;
    [SerializeField] private TextMeshProUGUI _cancelText;
    [SerializeField] private TextMeshProUGUI _pauseText;

    private void Awake()
    {
        _localInput = FindFirstObjectByType<LocalGameplayInput>();
    }

    private void Start()
    {
        GameServices.GameStateManager.StateChanged += OnGameStateChanged;
    }

    private void OnGameStateChanged(GameStateType state)
    {
        if(state != GameStateType.Paused)
        {
            gameObject.SetActive(false);
        }    
    }

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        if(_localInput.CurrentInputDevice is Gamepad)
        {
            RefreshTextsForGamepadInput();
        }
        else
        {
            RefreshTextsForMouseInput();
        }
    }

    private void RefreshTextsForGamepadInput()
    {
        Gamepad pad = Gamepad.current;
        _aimText.text = " left stick";
        _jumpAndShootText.text = " " + pad.buttonSouth.displayName;
        _skipText.text = " " + pad.buttonNorth.displayName;
        _openInventoryText.text = " " + pad.buttonWest.displayName;
        _cancelText.text = " " + pad.buttonEast.displayName;
        _pauseText.text = " " + pad.startButton.displayName;
        _showMapText.text = " right stick press";
    }

    private void RefreshTextsForMouseInput()
    {
        _aimText.text = " mouse";
        _jumpAndShootText.text = " left click";
        _skipText.text = " enter";
        _openInventoryText.text = " space";
        _cancelText.text = " right click";
        _pauseText.text = " esc";
        _showMapText.text = " M";
    }
}
