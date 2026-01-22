using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class SettingsPanelUI : MonoBehaviour
{
    [SerializeField] private SettingsController _settingsController;

    [SerializeField] private CheckBoxUI _invertControlsCheckbox;
    [SerializeField] private SliderInputUI _sfxSlider;
    [SerializeField] private SliderInputUI _musicSlider;
    [SerializeField] private HoverableSelectableContainerUI _firstElementHoverableUI;

    private LocalInputHandler _inputHandler;
    private void Awake()
    {
        _sfxSlider.ValueChanged += OnSfxSliderValueChanged;
        _musicSlider.ValueChanged += OnMusicSliderValueChanged;
        _invertControlsCheckbox.ValueChanged += OnInvertedControlsCheckboxValueChanged;
        _inputHandler = FindFirstObjectByType<LocalInputHandler>();
    }

    private void OnEnable()
    {
        StartCoroutine(SelectDefaultButtonNextFrame());
        _inputHandler.GameplayMenuDecrementValuePerformed += _invertControlsCheckbox.OnDecrementOrIncrementValuePerformed;
        _inputHandler.GameplayMenuIncrementValuePerformed += _invertControlsCheckbox.OnDecrementOrIncrementValuePerformed;
        _inputHandler.GameplayMenuDecrementValuePerformed += _sfxSlider.DecrementSliderValue;
        _inputHandler.GameplayMenuIncrementValuePerformed += _sfxSlider.IncrementSliderValue;
        _inputHandler.GameplayMenuDecrementValuePerformed += _musicSlider.DecrementSliderValue;
        _inputHandler.GameplayMenuIncrementValuePerformed += _musicSlider.IncrementSliderValue;
        _inputHandler.ToggleGameplayMenuPerformed += OnToggleGameplayMenuPerformed;
    }


    private void OnDisable()
    {
        _inputHandler.GameplayMenuDecrementValuePerformed -= _invertControlsCheckbox.OnDecrementOrIncrementValuePerformed;
        _inputHandler.GameplayMenuIncrementValuePerformed -= _invertControlsCheckbox.OnDecrementOrIncrementValuePerformed;
        _inputHandler.GameplayMenuDecrementValuePerformed -= _sfxSlider.DecrementSliderValue;
        _inputHandler.GameplayMenuIncrementValuePerformed -= _sfxSlider.IncrementSliderValue;
        _inputHandler.GameplayMenuDecrementValuePerformed -= _musicSlider.DecrementSliderValue;
        _inputHandler.GameplayMenuIncrementValuePerformed -= _musicSlider.IncrementSliderValue;
        _inputHandler.ToggleGameplayMenuPerformed -= OnToggleGameplayMenuPerformed;
    }
    private IEnumerator SelectDefaultButtonNextFrame()
    {
        yield return null;
        EventSystem.current.SetSelectedGameObject(_firstElementHoverableUI.gameObject);
    }

    private void Start()
    {
        _sfxSlider.SetInitialValue(_settingsController.GetSFXNormalizedVolume());
        _musicSlider.SetInitialValue(_settingsController.GetMusicNormalizedVolume());
        _invertControlsCheckbox.SetInitialValue(_settingsController.GetInvertedInput());
    }

    private void OnToggleGameplayMenuPerformed()
    {
        if (gameObject.activeSelf)
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
