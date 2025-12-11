using System.Collections.Generic;
using UnityEngine;

public static class CharacterAnimationStripParser
{

    private static readonly Dictionary<CharacterAnimationState, int[]> _stateIndexDict = new Dictionary<CharacterAnimationState, int[]>()
    {
        { CharacterAnimationState.Idle,  new[] { 0, 1 }},

        { CharacterAnimationState.PrepareToJump,  new[] { 3, 4 }},
        { CharacterAnimationState.BackFromLand,  new[] { 3 }},
        { CharacterAnimationState.Jump,  new[] { 3, 2 }},
        { CharacterAnimationState.Land,  new[] { 3, 4 }},

        { CharacterAnimationState.Hurt,  new[] { 5 }},
        { CharacterAnimationState.Death,  new[] { 6,7,8,9 }},

        { CharacterAnimationState.RangedAimMiddle,  new[] { 10 }},
        { CharacterAnimationState.RangedAimLow,  new[] { 11 }},
        { CharacterAnimationState.RangedAimHigh,  new[] { 12 }},

        { CharacterAnimationState.MeleeAimMiddle,  new[] { 13 }},
        { CharacterAnimationState.MeleeAimLow,  new[] { 18 }},
        { CharacterAnimationState.MeleeAimHigh,  new[] { 23 }},

        { CharacterAnimationState.MeleeAttackMiddle,  new[] { 13, 14, 15, 16, 17 }},
        { CharacterAnimationState.MeleeAttackLow,  new[] { 18, 19, 20, 21, 22}},
        { CharacterAnimationState.MeleeAttackHigh,  new[] { 23, 24, 25, 26, 27 }},
        { CharacterAnimationState.Guard,  new[] { 28 }},

    };

    public static Sprite[] ParseAnimation(Sprite[] strip, CharacterAnimationState state)
    {
        if (!_stateIndexDict.ContainsKey(state))
        {
            Debug.LogError($"Invalid {nameof(CharacterAnimationState)} when parsing animations from strips: {state}");
            return null;
        }

        return SelectSprites(strip, _stateIndexDict[state]);
    }

    private static Sprite[] SelectSprites(Sprite[] strip, int[] indexes)
    {
        var selected = new Sprite[indexes.Length];
        int i = 0;
        foreach(var index in indexes)
        {
            selected[i] = strip[index];
            i++;
        }
        return selected;
    }

}
