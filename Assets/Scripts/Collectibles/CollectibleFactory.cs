using System;
using Unity.VisualScripting;

public static class CollectibleFactory
{
    public static ICollectible CreateCollectible(CollectibleDefinition definition, bool asDrop = true)
    {
        if (definition.CollectibleType == CollectibleType.Item)
        {
            return CreateCollectible(definition as ItemDefinition, asDrop);
        }
        else if (definition.CollectibleType == CollectibleType.Effect)
        {
            return CreateCollectible(definition as EffectDefinition, asDrop);
        }
        else
        {
            throw new Exception($"Invalid definition type when creating collectibles: {definition.CollectibleType}");
        }
    }

    public static Item CreateCollectible(ItemDefinition definition, bool asDrop = true)
    {
        return new Item(definition, asDrop);
    }
    public static Effect CreateCollectible(EffectDefinition definition, bool asDrop = true)
    {
        return new Effect(definition, asDrop);
    }
}
