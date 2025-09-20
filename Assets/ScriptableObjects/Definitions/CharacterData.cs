using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Scriptable Objects/CharacterData")]
public class CharacterData : ScriptableObject
{
    public int MaxHealth;
    public float JumpStrength;
    public int ShootStrenth;
}
