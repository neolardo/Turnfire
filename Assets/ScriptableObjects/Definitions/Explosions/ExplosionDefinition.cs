using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileDefinition", menuName = "Scriptable Objects/Explosion/ExplosionDefinition")]
public class ExplosionDefinition : DatabaseItemScriptableObject
{
    public RangedStatFloat Force;
    public RangedStatFloat Radius;
    public AnimationDefinition Animation;
    public SFXDefiniton SFX;
}
