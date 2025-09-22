using UnityEngine;

[CreateAssetMenu(fileName = "ExplosionDefinitionSO", menuName = "Scriptable Objects/ExplosionData")]
public class ExplosionData : ScriptableObject
{
    public int Damage;
    public float ExplosionStrength;
    public float ExplosionRadius;}
