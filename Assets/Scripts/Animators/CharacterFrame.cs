using System;
using UnityEngine;

[Serializable]
public struct CharacterFrame
{
    public Sprite Base, Clothes, OverItemBase, OverItemClothes;

    public CharacterFrame(Sprite baseSprite, Sprite clothes, Sprite overItemBase, Sprite overItemClothes )
    {
        Base = baseSprite;
        Clothes = clothes;
        OverItemBase = overItemBase;
        OverItemClothes = overItemClothes;
    }

    public CharacterFrame(Sprite baseSprite, Sprite clothes)
    {
        Base = baseSprite;
        Clothes = clothes;
        OverItemBase = null;
        OverItemClothes = null;
    }
}
