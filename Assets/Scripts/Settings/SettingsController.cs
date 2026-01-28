using UnityEngine;
using UnityEngine.Audio;

public class SettingsController : MonoBehaviour
{
    [Header("Mixer")]
    [SerializeField] private AudioMixer _mainMixer;

    private const string _musicVolumeParam = "MusicVolume";
    private const string _sfxVolumeParam = "SFXVolume";

    private const string MusicPrefKey = "MusicVolume";
    private const string SfxPrefKey = "SFXVolume";
    private const string InvertInputPrefKey = "InvertedInput";
    private const int _trueValue = 1;

    private const float MinDb = -80f; 
    private const float MaxDb = 0f;
    private const float DecibelShapeExponent = 0.3f;

    private void Start()
    {
        LoadAndApplyVolumes();
    }

    #region Audio
    public void SetMusicVolume(float normalizedValue)
    {
        SetVolume(_musicVolumeParam, normalizedValue);
        PlayerPrefs.SetFloat(MusicPrefKey, normalizedValue);
    }
    public float GetMusicNormalizedVolume()
    {
        return PlayerPrefs.GetFloat(MusicPrefKey);
    }

    public void SetSFXVolume(float normalizedValue)
    {
        SetVolume(_sfxVolumeParam, normalizedValue);
        PlayerPrefs.SetFloat(SfxPrefKey, normalizedValue);
    }

    public float GetSFXNormalizedVolume()
    {
        return PlayerPrefs.GetFloat(SfxPrefKey);
    }

    private void SetVolume(string parameter, float normalizedValue)
    {
        float db = NormalizedToDecibels(normalizedValue);
        _mainMixer.SetFloat(parameter, db);
    }

    private void LoadAndApplyVolumes()
    {
        float music = PlayerPrefs.GetFloat(MusicPrefKey, 1f);
        float sfx = PlayerPrefs.GetFloat(SfxPrefKey, 1f);

        SetVolume(_musicVolumeParam, music);
        SetVolume(_sfxVolumeParam, sfx);
    }

    private static float NormalizedToDecibels(float normalizedValue)
    {
        if (normalizedValue <= 0f)
            return MinDb;

        float shaped = Mathf.Pow(normalizedValue, DecibelShapeExponent);
        return Mathf.Lerp(MinDb, MaxDb, shaped);
    }

    #endregion

    #region Inverted Input

    public void SetInvertedInput(bool isInputInverted)
    {
        PlayerPrefs.SetInt(InvertInputPrefKey, isInputInverted ? _trueValue : 0);
    }
    public bool GetInvertedInput()
    {
        return PlayerPrefs.GetInt(InvertInputPrefKey) == _trueValue;
    }

    #endregion
}