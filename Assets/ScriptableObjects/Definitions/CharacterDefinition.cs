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

    [Header("SFXs")]

    public SFXDefiniton InAirSFX;
    public SFXDefiniton PrepareToJumpSFX;
    public SFXDefiniton LandSFX;
    public SFXDefiniton CancelJumpSFX;
    public SFXDefiniton HurtSFX;
    public SFXDefiniton DeathSFX;
    public SFXDefiniton AimStartSFX;
    public SFXDefiniton AimCancelSFX;

    [Header("Stats")]
    public int MaxHealth;
    [Range(Constants.MinJumpStrength, Constants.MaxJumpStrength)] public float JumpStrength = Constants.MinJumpStrength; //TODO: ranged stat instead?
    public List<ItemDefinition> InitialItems;
}
