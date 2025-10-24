using UnityEngine;

public class DestructibleTerrainReference : MonoBehaviour
{
    [SerializeField] private DestructibleTerrainManager _manager;
    public void ApplyExplosion(Vector2 position, float radius)
    {
        _manager.ApplyExplosion(position, radius);
    }

}
