using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDefinitionSO", menuName = "Scriptable Objects/WeaponData")]
public class WeaponData : ItemData
{
    public RangedStatFloat FireStrength;
    public int NumProjectiles;
}
