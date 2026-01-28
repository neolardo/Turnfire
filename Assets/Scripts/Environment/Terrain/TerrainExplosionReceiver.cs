using UnityEngine;

public class TerrainExplosionReceiver : MonoBehaviour
{
    [SerializeField] private TerrainManager _manager;
    public void ApplyExplosion(Vector2 position, float radius)
    {
        _manager.RegisterExplosion(position, radius);
    }

}
