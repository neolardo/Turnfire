using UnityEngine;
using UnityEngine.EventSystems;

public class MenuSettingsUI : MonoBehaviour
{
    [SerializeField] private SettingsController _settingsController;

    [SerializeField] private MenuButtonUI _backButton;
    [SerializeField] private CheckBoxUI _invertControlsCheckbox;
    [SerializeField] private SliderInputUI _sfxSlider;
    [SerializeField] private SliderInputUI _musicSlider;
    private MenuUIManager _menuUIManager;
    private LocalMenuUIInputSource _inputManager;
    private void Awake()
    {
        _backButton.ButtonPressed += OnBackButtonPressed;
        _sfxSlider.ValueChanged += OnSfxSliderValueChanged;
        _musicSlider.ValueChanged += OnMusicSliderValueChanged;
        _invertControlsCheckbox.ValueChanged += OnInvertedControlsCheckboxValueChanged;
        _inputManager = FindFirstObjectByType<LocalMenuUIInputSource>();
        _menuUIManager = FindFirstObjectByType<MenuUIManager>();
    }

    private void OnEnable()
    {
        _inputManager.MenuDecrementValuePerformed += _invertControlsCheckbox.OnDecrementOrIncrementValuePerformed;
        _inputManager.MenuIncrementValuePerformed += _invertControlsCheckbox.OnDecrementOrIncrementValuePerformed;
        _inputManager.MenuDecrementValuePerformed += _sfxSlider.DecrementSliderValue;
        _inputManager.MenuIncrementValuePerformed += _sfxSlider.IncrementSliderValue;
        _inputManager.MenuDecrementValuePerformed += _musicSlider.DecrementSliderValue;
        _inputManager.MenuIncrementValuePerformed += _musicSlider.IncrementSliderValue;
    }

    private void OnDisable()
    {
        _inputManager.MenuDecrementValuePerformed -= _invertControlsCheckbox.OnDecrementOrIncrementValuePerformed;
        _inputManager.MenuIncrementValuePerformed -= _invertControlsCheckbox.OnDecrementOrIncrementValuePerformed;
        _inputManager.MenuDecrementValuePerformed -= _sfxSlider.DecrementSliderValue;
        _inputManager.MenuIncrementValuePerformed -= _sfxSlider.IncrementSliderValue;
        _inputManager.MenuDecrementValuePerformed -= _musicSlider.DecrementSliderValue;
        _inputManager.MenuIncrementValuePerformed -= _musicSlider.IncrementSliderValue;
    }

    private void Start()
    {
        _sfxSlider.SetInitialValue(_settingsController.GetSFXNormalizedVolume());
        _musicSlider.SetInitialValue(_settingsController.GetMusicNormalizedVolume());
        _invertControlsCheckbox.SetInitialValue(_settingsController.GetInvertedInput());
        EventSystem.current.SetSelectedGameObject(_sfxSlider.gameObject);
    }

    private void OnSfxSliderValueChanged(float value)
    {
        _settingsController.SetSFXVolume(value);
    }

    private void OnMusicSliderValueChanged(float value)
    {
        _settingsController.SetMusicVolume(value);
    }

    private void OnInvertedControlsCheckboxValueChanged(bool value)
    {
        _settingsController.SetInvertedInput(value);
    }

    private void OnBackButtonPressed()
    {
        _menuUIManager.SwitchToPreviousPanel();
    }
}

