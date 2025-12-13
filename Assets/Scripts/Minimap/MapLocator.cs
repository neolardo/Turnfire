using UnityEngine;

// helper for bot simulations
public class MapLocator : MonoBehaviour
{
    public MapDefinition Map0;
    public MapDefinition Map1;
    public MapDefinition Map2;

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
}
