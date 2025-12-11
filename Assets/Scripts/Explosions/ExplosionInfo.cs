using System.Collections.Generic;

public readonly struct ExplosionInfo
{
    public readonly IEnumerable<Character> ExplodedCharacters;
    public readonly Projectile Source;
    public readonly Explosion Explosion;

    // stats
    public readonly int Damage;

    public ExplosionInfo(int damage, IEnumerable<Character> explodedCharacters, Projectile source, Explosion explosion)
    {
        Damage = damage;
        ExplodedCharacters = explodedCharacters;
        Source = source;
        Explosion = explosion;
    }
}
