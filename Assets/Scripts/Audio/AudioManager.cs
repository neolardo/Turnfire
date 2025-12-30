using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Mixer")]
    [SerializeField] private AudioMixer _mixer;
    [SerializeField] private AudioMixerGroup _SFXGroup;
    [SerializeField] private AudioMixerGroup _UISoundsGroup;

    [Header("Sources")]
    [SerializeField] private AudioSource _uiSource;
    [SerializeField] private AudioSourcePool SFXPool;

    private void Awake()
    {
        if (Instance != null) 
        {
            Destroy(gameObject); 
            return;
        }
        Instance = this;
        _uiSource.outputAudioMixerGroup = _UISoundsGroup;
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySFXAt(SFXDefiniton sfx, Vector2 position)
    {
        //SFXPool.PlayOnAny(sfx.GetRandomClip(), position);
    }

    public void PlaySFXAt(SFXDefiniton sfx, Transform transform)
    {
        //SFXPool.PlayOnAny(sfx.GetRandomClip(), transform);
    }

    public void PlayUISound(SFXDefiniton sfx)
    {
        //_uiSource.clip = sfx.GetRandomClip();
        //_uiSource.Play();
    }
}
