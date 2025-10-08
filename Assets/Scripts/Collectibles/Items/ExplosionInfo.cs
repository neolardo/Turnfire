using System.Collections.Generic;

public readonly struct ExplosionInfo
{
    public readonly IEnumerable<Character> ExplodedCharacters;
    public readonly Projectile Source;

    public ExplosionInfo(IEnumerable<Character> explodedCharacters, Projectile source)
    {
        ExplodedCharacters = explodedCharacters;
        Source = source;
    }
}
