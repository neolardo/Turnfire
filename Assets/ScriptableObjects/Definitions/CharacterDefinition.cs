using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterDefinition", menuName = "Scriptable Objects/CharacterDefinition")]
public class CharacterDefinition : ScriptableObject
{
    [Header("Animations")]
    public CharacterAnimationDefinition IdleAnimationDefinition;
    public CharacterAnimationDefinition JumpAnimationDefinition;
    public CharacterAnimationDefinition LandAnimationDefinition;
    public CharacterAnimationDefinition PrepareToJumpAnimationDefinition;
    public CharacterAnimationDefinition HurtAnimationDefinition;
    public CharacterAnimationDefinition DeathAnimationDefinition;
    public CharacterAnimationDefinition AimLowAnimationDefinition;
    public CharacterAnimationDefinition AimMiddleAnimationDefinition;
    public CharacterAnimationDefinition AimHighAnimationDefinition;
    public CharacterAnimationDefinition BackFromLandAnimationDefinition;

    [Header("Stats")]
    public int MaxHealth;
    public float JumpStrength;
    public List<ItemDefinition> InitialItems;
}
