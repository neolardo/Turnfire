using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "SFXDefinition", menuName = "Scriptable Objects/Audio/SFXDefinition")]
public class SFXDefiniton : ScriptableObject
{
    public AudioClip[] clips;

    private int _lastClipIndex;

    public AudioClip GetRandomClip()
    {
        int index = 0;
        if (clips.Length > 1)
        {
            var indexes = new List<int>(Enumerable.Range(0, clips.Length));
            indexes.Remove(_lastClipIndex);
            index = indexes[UnityEngine.Random.Range(0, indexes.Count)]; 
        }
        _lastClipIndex = index;

        return clips[index];
    }
}
