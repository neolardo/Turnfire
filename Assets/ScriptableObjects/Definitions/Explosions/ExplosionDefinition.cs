using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileDefinition", menuName = "Scriptable Objects/ExplosionDefinition")]
public class ExplosionDefinition : ScriptableObject
{
    public RangedStatFloat Force;
    public RangedStatFloat Radius;
    public AnimationDefinition Animation;
    public SFXDefiniton SFX;
}
