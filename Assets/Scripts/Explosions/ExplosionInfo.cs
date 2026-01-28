using System.Collections.Generic;

public readonly struct ExplosionInfo
{
    public readonly IEnumerable<Character> ExplodedCharacters;
    public readonly IExplosion Explosion; 

    public ExplosionInfo(IEnumerable<Character> explodedCharacters,IExplosion explosion)
    {
        ExplodedCharacters = explodedCharacters;
        Explosion = explosion;
    }
}
