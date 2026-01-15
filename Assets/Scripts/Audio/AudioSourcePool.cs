using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioSourcePool : PoolBase<AudioSource>
{
    [SerializeField] private AudioMixerGroup _mixerOutputGroup;

    protected override AudioSource CreateInstance()
    {
        var item = base.CreateInstance();
        item.outputAudioMixerGroup = _mixerOutputGroup;
        return item;
    }
    public void PlayOnAny(AudioClip clip, Vector2 position)
    {
        var source = Get();
        source.clip = clip;
        source.transform.position = position;
        source.Play();
        StartCoroutine(ReleaseWhenFinishedPlaying(source));
    }

    public void PlayOnAny(AudioClip clip, Transform transform)
    {
        var source = Get();
        source.clip = clip;
        source.transform.SetParent(transform);
        source.transform.localPosition = Vector3.zero;
        source.Play();
        StartCoroutine(ReleaseWhenFinishedPlaying(source));
    }

    private IEnumerator ReleaseWhenFinishedPlaying(AudioSource source)
    {
        if(source == null)
        {
            yield break;
        }
        yield return new WaitWhile(() => source != null && source.isPlaying);

        Release(source);
    }
}
