using UnityEngine;

[CreateAssetMenu(fileName = "CharacterAnimatorDefinition", menuName = "Scriptable Objects/Animators/CharacterAnimatorDefinition")]
public class CharacterAnimatorDefinition : ScriptableObject
{
    [Header("Colors")]
    public Color HurtFlashColor = Color.red;
    public Color HealFlashColor = Color.green;
    public Color ItemFlashColor = new Color(.8f,.8f,.8f, 0.3f);

    [Header("Durations")]
    public float IdleAnimationFrameDuration;
    public float HurtAnimationFrameDuration;
    public float DeathAnimationFrameDuration;
    public float FlyAnimationFrameDuration;
    public float ItemActionAnimationFrameDuration;
    public float GuardActionAnimationFrameDuration;

    public float ItemUsageDelay;

    public float HurtAnimationFlashInSeconds;
    public float HurtAnimationFlashOutSeconds;

    public float HealAnimationFlashInSeconds;
    public float HealAnimationFlashOutSeconds;

    public float ItemFlashInSeconds;
    public float ItemFlashOutSeconds;

}
