using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileData", menuName = "Scriptable Objects/ProjectileData")]
public class ProjectileData : ScriptableObject
{
    public int Damage;
    public float FireStrength;
    public float ExplosionStrength;
    public float ExplosionRadius;
}
