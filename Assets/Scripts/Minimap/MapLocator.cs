using UnityEngine;

public class MapLocator : MonoBehaviour
{
    [SerializeField] private MapDefinition Map0;
    [SerializeField] private MapDefinition Map1;
    [SerializeField] private MapDefinition Map2;

    public MapDefinition GetMap(int mapIndex)
    {
        switch (mapIndex)
        {
            case 0: return Map0;
            case 1: return Map1;
            case 2: return Map2;
            default:
                throw new System.Exception("Invalid map index requested.");
        }
    }

    public MapDefinition GetMap(string sceneName)
    {
        switch(sceneName.ToLower())
        {
            case "map0": return Map0;
            case "map1": return Map1;
            case "map2": return Map2;
            default:
                throw new System.Exception("Invalid map scecne name requested.");
        }
    }


}
