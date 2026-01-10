public class CannonBallProjectileBehavior : BallisticProjectileBehavior
{
    private CannonBallProjectileDefinition _definition;

    public CannonBallProjectileBehavior(CannonBallProjectileDefinition definition) : base(definition)
    { 
        _definition = definition;
    }

}
