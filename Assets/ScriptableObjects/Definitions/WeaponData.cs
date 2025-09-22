using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDefinitionSO", menuName = "Scriptable Objects/WeaponData")]
public class WeaponData : ScriptableObject
{
    public float FireStrength;
    public int NumProjectiles;
}
