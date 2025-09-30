using UnityEngine;

[CreateAssetMenu(fileName = "ExplosionDefinitionSO", menuName = "Scriptable Objects/ExplosionData")]
public class ExplosionData : ScriptableObject
{
    public RangedStatInt Damage;
    public RangedStatFloat ExplosionStrength;
    public RangedStatFloat ExplosionRadius;
}
