using UnityEngine;

[CreateAssetMenu(fileName = "MapDefinition", menuName = "Scriptable Objects/MapDefinition")]
public class MapDefinition : ScriptableObject
{
    public string SceneName;
    public Minimap[] Minimaps;
}
