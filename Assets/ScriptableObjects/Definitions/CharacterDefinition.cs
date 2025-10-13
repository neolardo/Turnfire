using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterDefinition", menuName = "Scriptable Objects/CharacterDefinition")]
public class CharacterDefinition : ScriptableObject
{
    [Header("Animator")]
    public CharacterFrame[] IdleFrames;
    public CharacterFrame[] JumpFrames;
    public CharacterFrame[] LandFrames;
    public CharacterFrame[] HurtFrames;
    public CharacterFrame[] DeathFrames;
    public CharacterFrame AimMiddleFrame;
    public CharacterFrame AimLowFrame;
    public CharacterFrame AimHighFrame;
    public CharacterFrame[] BackFromLandFrames;

    [Header("Stats")]
    public int MaxHealth;
    public float JumpStrength;
    public List<ItemDefinition> InitialItems;
}
