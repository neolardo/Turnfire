using UnityEngine;

[CreateAssetMenu(fileName = "MusicDefinition", menuName = "Scriptable Objects/Audio/MusicDefinition")]
public class MusicDefinition : ScriptableObject
{
    public MusicType Type;
    public AudioClip Clip;
}
