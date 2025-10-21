public class CannonBallProjectileBehavior : SimpleProjectileBehavior
{
    private CannonBallProjectileDefinition _definition;

    public CannonBallProjectileBehavior(CannonBallProjectileDefinition definition) : base(definition)
    { 
        _definition = definition;
    }

}
