using System;

public static class CollectibleFactory
{
    public static ICollectible CreateCollectible(CollectibleDefinition definition)
    {
        if (definition.Type == CollectibleType.Item)
        {
            return CreateCollectible(definition as ItemDefinition);
        }
        else if (definition.Type == CollectibleType.Effect)
        {
            return CreateCollectible(definition as EffectDefinition);
        }
        else
        {
            throw new Exception($"Invalid definition type when creating collectibles: {definition.Type}");
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
