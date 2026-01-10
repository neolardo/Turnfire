using UnityEngine;

[CreateAssetMenu(fileName = "AnimationDefinition", menuName = "Scriptable Objects/Animations/AnimationDefinition")]
public class AnimationDefinition : ScriptableObject
{
    public Sprite[] Frames;

    public float GetTotalDuration(float frameDuration) => frameDuration * Frames.Length;
}
