using UnityEngine;

[CreateAssetMenu(fileName = "MapDefinition", menuName = "Scriptable Objects/MapDefinition")]
public class MapDefinition : ScriptableObject
{
    [Header("Scene")]
    public string SceneName;
    public Minimap[] Minimaps;

    [Header("Drops")]
    public float FirstDropChance;
    public float MultipleDropChance;
    public int MaxDropsPerRound;
    public ItemDrop[] PossibleDrops;
}
