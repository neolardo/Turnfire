using UnityEngine;
using UnityEngine.EventSystems;

public class MenuSettingsUI : MonoBehaviour
{
    [SerializeField] private SettingsController _settingsController;

    [SerializeField] private MenuButtonUI _backButton;
    [SerializeField] private MenuCheckBoxUI _invertControlsCheckbox;
    [SerializeField] private SliderInputUI _sfxSlider;
    [SerializeField] private SliderInputUI _musicSlider;
    private MenuUIManager _menuUIManager;
    private void Awake()
    {
        _backButton.ButtonPressed += OnBackButtonPressed;
        _sfxSlider.ValueChanged += OnSfxSliderValueChanged;
        _musicSlider.ValueChanged += OnMusicSliderValueChanged;
        _invertControlsCheckbox.ValueChanged += OnInvertedControlsCheckboxValueChanged;
        _menuUIManager = FindFirstObjectByType<MenuUIManager>();
    }

    private void Start()
    {
        _sfxSlider.SetInitialValue(_settingsController.GetSFXNormalizedVolume());
        _musicSlider.SetInitialValue(_settingsController.GetMusicNormalizedVolume());
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

