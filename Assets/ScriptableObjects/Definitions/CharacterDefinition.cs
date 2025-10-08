using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterDefinition", menuName = "Scriptable Objects/CharacterDefinition")]
public class CharacterDefinition : ScriptableObject
{
    public int MaxHealth;
    public float JumpStrength;
    public List<ItemDefinition> InitialItems;
}
