using System;

public static class CollectibleFactory
{
    public static ICollectible CreateCollectible(CollectibleDefinition definition)
    {
        if (definition.CollectibleType == CollectibleType.Item)
        {
            return CreateCollectible(definition as ItemDefinition);
        }
        else if (definition.CollectibleType == CollectibleType.Effect)
        {
            return CreateCollectible(definition as EffectDefinition);
        }
        else
        {
            throw new Exception($"Invalid definition type when creating collectibles: {definition.CollectibleType}");
        }
    }

    public static Item CreateCollectible(ItemDefinition definition)
    {
        return new Item(definition);
    }
    public static Effect CreateCollectible(EffectDefinition definition)
    {
        return new Effect(definition);
    }
}
