using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class ArmorDefinition : ItemDefinition
{
    public override ItemType ItemType => ItemType.Armor;

    public RangedStatInt MaxDurability;
    public abstract bool IsProtective { get; }

    public Sprite[] EquippedSpriteStrip;
    public Dictionary<CharacterAnimationState, Sprite[]> Animations { get; private set; }

    public override IEnumerable<RangedStat> GetRangedStats()
    {
        return new[] { MaxDurability };
    }

    public void InitializeAnimations()
    {
        Animations = new Dictionary<CharacterAnimationState, Sprite[]>();
        var allStates = Enum.GetValues(typeof(CharacterAnimationState)).Cast<CharacterAnimationState>().ToList();
        allStates.Remove(CharacterAnimationState.None);

        foreach (var state in allStates)
        {
            var sprites = CharacterAnimationStripParser.ParseAnimation(EquippedSpriteStrip, state);
            Animations[state] = sprites;
        }
    }

}
