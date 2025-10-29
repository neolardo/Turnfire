using System;

public class Effect : ICollectible
{
    public EffectDefinition Definition { get; }

    public event Action<ICollectible> CollectibleDestroyed;

    public Effect(EffectDefinition definition, bool asDrop = true)
    {
        Definition = definition;
    }


    public bool TryCollect(Character c)
    {
        //TODO
        return false;
    }
}
