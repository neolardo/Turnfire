using System.Collections.Generic;

public readonly struct ExplosionInfo
{
    public readonly IEnumerable<Character> ExplodedCharacters;
    public readonly Projectile Source;
    public readonly Explosion Explosion;

    public ExplosionInfo(IEnumerable<Character> explodedCharacters, Projectile source, Explosion explosion)
    {
        ExplodedCharacters = explodedCharacters;
        Source = source;
        Explosion = explosion;
    }
}
