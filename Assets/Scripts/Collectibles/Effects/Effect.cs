public class Effect : ICollectible
{
    public EffectDefinition Definition { get; }
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
