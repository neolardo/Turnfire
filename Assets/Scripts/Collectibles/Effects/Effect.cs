using System;

public class Effect : ICollectible
{
    public EffectDefinition Definition { get; }

    public event Action<ICollectible> CollectibleDestroyed;

    public Effect(EffectDefinition definition)
    {
        Definition = definition;
    }


    public bool TryCollect(Character c)
    {
        //TODO
        return false;
    }
}
