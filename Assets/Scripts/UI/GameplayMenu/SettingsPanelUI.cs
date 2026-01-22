using UnityEngine;
using UnityEngine.EventSystems;

public class SettingsPanelUI : MonoBehaviour
{
    [SerializeField] private SettingsController _settingsController;

    [SerializeField] private CheckBoxUI _invertControlsCheckbox;
    [SerializeField] private SliderInputUI _sfxSlider;
    [SerializeField] private SliderInputUI _musicSlider;
    private void Awake()
    {
        _sfxSlider.ValueChanged += OnSfxSliderValueChanged;
        _musicSlider.ValueChanged += OnMusicSliderValueChanged;
        _invertControlsCheckbox.ValueChanged += OnInvertedControlsCheckboxValueChanged;
    }

    private void OnEnable()
    {
        //TODO
        //_inputManager.MenuDecrementValuePerformed += _invertControlsCheckbox.OnDecrementOrIncrementValuePerformed;
        //_inputManager.MenuIncrementValuePerformed += _invertControlsCheckbox.OnDecrementOrIncrementValuePerformed;
        //_inputManager.MenuDecrementValuePerformed += _sfxSlider.DecrementSliderValue;
        //_inputManager.MenuIncrementValuePerformed += _sfxSlider.IncrementSliderValue;
        //_inputManager.MenuDecrementValuePerformed += _musicSlider.DecrementSliderValue;
        //_inputManager.MenuIncrementValuePerformed += _musicSlider.IncrementSliderValue;
    }

    private void OnDisable()
    {
        //_inputManager.MenuDecrementValuePerformed -= _invertControlsCheckbox.OnDecrementOrIncrementValuePerformed;
        //_inputManager.MenuIncrementValuePerformed -= _invertControlsCheckbox.OnDecrementOrIncrementValuePerformed;
        //_inputManager.MenuDecrementValuePerformed -= _sfxSlider.DecrementSliderValue;
        //_inputManager.MenuIncrementValuePerformed -= _sfxSlider.IncrementSliderValue;
        //_inputManager.MenuDecrementValuePerformed -= _musicSlider.DecrementSliderValue;
        //_inputManager.MenuIncrementValuePerformed -= _musicSlider.IncrementSliderValue;
    }

    private void Start()
    {
        _sfxSlider.SetInitialValue(_settingsController.GetSFXNormalizedVolume());
        _musicSlider.SetInitialValue(_settingsController.GetMusicNormalizedVolume());
        _invertControlsCheckbox.SetInitialValue(_settingsController.GetInvertedInput());
        EventSystem.current.SetSelectedGameObject(_sfxSlider.gameObject);
        if (GameServices.IsInitialized)
        {
            OnGameServicesInitialized();
        }
        else
        {
            GameServices.Initialized += OnGameServicesInitialized;
        }
    }

    private void OnGameServicesInitialized()
    {
        GameServices.GameStateManager.StateChanged += OnGameStateChanged;
    }

    private void OnDestroy()
    {
        if(GameServices.GameStateManager != null)
        {
            GameServices.GameStateManager.StateChanged -= OnGameStateChanged;
        }
    }
    //TODO: gradient sky shader?

    private void OnGameStateChanged(GameStateType state)
    {
        if (state != GameStateType.Paused)
        {
            gameObject.SetActive(false);
        }
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
}
