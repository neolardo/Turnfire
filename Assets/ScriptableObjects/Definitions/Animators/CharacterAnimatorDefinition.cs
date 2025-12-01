using UnityEngine;

[CreateAssetMenu(fileName = "CharacterAnimatorDefinition", menuName = "Scriptable Objects/Animators/CharacterAnimatorDefinition")]
public class CharacterAnimatorDefinition : ScriptableObject
{
    public float IdleAnimationFrameDuration;
    public float HurtAnimationFrameDuration;
    public float DeathAnimationFrameDuration;
    public float FlyAnimationFrameDuration;
    public float ItemActionAnimationFrameDuration;
    public float GuardActionAnimationFrameDuration;
    public float ItemUsageDelay;
}
