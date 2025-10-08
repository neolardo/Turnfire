using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileDefinition", menuName = "Scriptable Objects/ExplosionDefinition")]
public class ExplosionDefinition : ScriptableObject
{
    public RangedStatInt Damage;
    public RangedStatFloat ExplosionForce;
    public RangedStatFloat ExplosionRadius;
}
