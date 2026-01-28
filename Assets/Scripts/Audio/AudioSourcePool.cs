using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioSourcePool : PoolBase<PoolableAudioSource>
{
    [SerializeField] private AudioMixerGroup _mixerOutputGroup;

    protected override PoolableAudioSource CreateInstance()
    {
        var item = base.CreateInstance();
        item.Source.outputAudioMixerGroup = _mixerOutputGroup;
        return item;
    }
    public void PlayOnAny(AudioClip clip, Vector2 position)
    {
        var item = Get();
        item.Source.clip = clip;
        item.Source.transform.position = position;
        item.Source.Play();
        StartCoroutine(ReleaseWhenFinishedPlaying(item));
    }

    public void PlayOnAny(AudioClip clip, Transform transform)
    {
        var item = Get();
        item.Source.clip = clip;
        item.transform.SetParent(transform);
        item.transform.localPosition = Vector3.zero;
        item.Source.Play();
        StartCoroutine(ReleaseWhenFinishedPlaying(item));
    }

    private IEnumerator ReleaseWhenFinishedPlaying(PoolableAudioSource item)
    {
        if(item == null)
        {
            yield break;
        }
        yield return new WaitWhile(() => item.Source != null && item.Source.isPlaying);

        Release(item);
    }
}
