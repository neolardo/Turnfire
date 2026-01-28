public interface IDamageSourceDefinition
{
    DamageSourceType Type { get; }
    int SourceDefinitionId { get;}
    SFXDefiniton HitSFX { get; }
}
