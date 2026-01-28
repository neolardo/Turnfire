using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PoolableAudioSource : SimplePoolable
{
    public AudioSource Source { get; private set; }

    private void Awake()
    {
        Source = GetComponent<AudioSource>();
    }
}
