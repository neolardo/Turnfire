using UnityEngine;

[CreateAssetMenu(fileName = "CharacterAnimatorDefinition", menuName = "Scriptable Objects/Animators/CharacterAnimatorDefinition")]
public class CharacterAnimatorDefinition : ScriptableObject
{
    [Header("Sprite Animations")]
    public float IdleAnimationFrameDuration;
    public float HurtAnimationFrameDuration;
    public float DeathAnimationFrameDuration;
    public float FlyAnimationFrameDuration;
    public float ItemActionAnimationFrameDuration;
    public float GuardActionAnimationFrameDuration;
    public float MeleeAttackAnimationFrameDuration;

    [Header("Flash Animations")]
    public Color HurtFlashColor = Color.red;
    public Color HealFlashColor = Color.green;
    public Color ItemFlashColor = new Color(.8f, .8f, .8f, 0.3f);

    public float HurtAnimationFlashInSeconds;
    public float HurtAnimationFlashOutSeconds;

    public float HealAnimationFlashInSeconds;
    public float HealAnimationFlashOutSeconds;

    public float ItemFlashInSeconds;
    public float ItemFlashOutSeconds;

    [Header("Healtbar")]
    public float HealtbarSlideInSeconds;
    public float HealtbarSlideOutSeconds;

}
