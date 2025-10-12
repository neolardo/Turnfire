using UnityEngine;

[CreateAssetMenu(fileName = "CharacterAnimatorDefinition", menuName = "Scriptable Objects/Animators/CharacterAnimatorDefinition")]
public class CharacterAnimatorDefinition : ScriptableObject
{
    public float IdleAnimationDurationPerFrame;
    public float HurtAnimationDurationPerFrame;
    public float DeathAnimationDurationPerFrame;
    public float FlyAnimationDurationPerFrame;
    public float ItemUsageDelay;
}
