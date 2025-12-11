using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public readonly struct CharacterAnimationFrame
{
    public readonly Dictionary<CharacterAnimationLayer, Sprite> Sprites;

    public CharacterAnimationFrame(Sprite body, Sprite head, Sprite clothes, Sprite overItemBody, Sprite overItemClothes )
    {
        Sprites = new Dictionary<CharacterAnimationLayer, Sprite>();
        Sprites[CharacterAnimationLayer.Body] = body;
        Sprites[CharacterAnimationLayer.Head] = head;
        Sprites[CharacterAnimationLayer.Clothes] = clothes;
        Sprites[CharacterAnimationLayer.OverItemBody] = overItemBody;
        Sprites[CharacterAnimationLayer.OverItemClothes] = overItemClothes;
    }
}
