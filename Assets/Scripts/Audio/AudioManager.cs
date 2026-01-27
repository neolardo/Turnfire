using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Mixer")]
    [SerializeField] private AudioMixer _mixer;
    [SerializeField] private AudioMixerGroup _MusicGroup;
    [SerializeField] private AudioMixerGroup _SFXGroup;
    [SerializeField] private AudioMixerGroup _UISoundsGroup;

    [Header("Sources")]
    [SerializeField] private AudioSource _uiSource;
    [SerializeField] private AudioSource _musicSourceA;
    [SerializeField] private AudioSource _musicSourceB;
    [SerializeField] private AudioSourcePool _SFXPool;

    [Header("Music")]
    [SerializeField] private float _musicFadeDuration = 2f;

    private AudioSource _activeMusicSource;
    private Coroutine _musicFadeCoroutine;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _uiSource.outputAudioMixerGroup = _UISoundsGroup;
        _musicSourceA.outputAudioMixerGroup = _MusicGroup;
        _musicSourceB.outputAudioMixerGroup = _MusicGroup;
        _activeMusicSource = _musicSourceA;
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (clip == null)
            return;

        AudioSource newSource = _activeMusicSource == _musicSourceA
            ? _musicSourceB
            : _musicSourceA;

        newSource.clip = clip;
        newSource.loop = loop;
        newSource.volume = 0f;
        newSource.Play();

        if (_musicFadeCoroutine != null)
            StopCoroutine(_musicFadeCoroutine);

        _musicFadeCoroutine = StartCoroutine(CrossFadeMusic(_activeMusicSource, newSource));

        _activeMusicSource = newSource;
    }

    private IEnumerator CrossFadeMusic(AudioSource from, AudioSource to)
    {
        float time = 0f;

        while (time < _musicFadeDuration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / _musicFadeDuration;

            if (from != null)
                from.volume = Mathf.Lerp(1f, 0f, t);

            to.volume = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }

        if (from != null)
        {
            from.Stop();
            from.clip = null;
            from.volume = 0f;
        }

        to.volume = 1f;
    }

    public void PlaySFXAt(SFXDefiniton sfx, Vector2 position)
    {
        _SFXPool.PlayOnAny(sfx.GetRandomClip(), position);
    }

    public void PlaySFXAt(SFXDefiniton sfx, Transform transform)
    {
        _SFXPool.PlayOnAny(sfx.GetRandomClip(), transform);
    }

    public void PlayUISound(SFXDefiniton sfx)
    {
        _uiSource.clip = sfx.GetRandomClip();
        _uiSource.Play();
    }
}
