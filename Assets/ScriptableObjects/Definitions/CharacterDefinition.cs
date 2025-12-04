using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterDefinition", menuName = "Scriptable Objects/CharacterDefinition")]
public class CharacterDefinition : ScriptableObject
{
    [Header("Animations")]
    public Sprite[] BodySpriteStrip;
    public Sprite[] HeadSpriteStrip;
    public Sprite[] ClothesSpriteStrip;
    public Sprite[] OverItemBodySpriteStrip;
    public Sprite[] OverItemClothesSpriteStrip;
    public Color HurtFlashColor = Color.red;
    public Color HealFlashColor = Color.green;
    public Color ItemEquipFlashColor = Color.white;

    public Dictionary<CharacterAnimationState, CharacterAnimationFrame[]> Animations { get; private set; }

    [Header("SFX")]

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
    public const float JumpStrength = Constants.DefaultJumpStrength;
    public List<ItemDefinition> InitialItems;


    public void InitializeAnimations()
    {
        Animations = new Dictionary<CharacterAnimationState, CharacterAnimationFrame[]>();
        var allStates = Enum.GetValues(typeof(CharacterAnimationState)).Cast<CharacterAnimationState>().ToList();
        allStates.Remove(CharacterAnimationState.None);

        foreach (var state in allStates)
        {
            var bodySprites = CharacterAnimationStripParser.ParseAnimation(BodySpriteStrip, state);
            var headSprites = CharacterAnimationStripParser.ParseAnimation(HeadSpriteStrip, state);
            var clothesSprites = CharacterAnimationStripParser.ParseAnimation(ClothesSpriteStrip, state);
            var overItemBodySprites = CharacterAnimationStripParser.ParseAnimation(OverItemBodySpriteStrip, state);
            var overItemClothesSprites = CharacterAnimationStripParser.ParseAnimation(OverItemClothesSpriteStrip, state);
            int spriteCount = bodySprites.Length;
            Animations[state] = new CharacterAnimationFrame[spriteCount];
            for (int frame = 0; frame < spriteCount; frame++)
            {
                Animations[state][frame] = new CharacterAnimationFrame(bodySprites[frame], headSprites[frame], clothesSprites[frame], overItemBodySprites[frame], overItemClothesSprites[frame]);
            }
        }
    }
}
