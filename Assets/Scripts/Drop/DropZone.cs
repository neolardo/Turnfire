using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class DropZone : MonoBehaviour
{
    private BoxCollider2D _zoneCollider;

    private void Awake()
    {
        _zoneCollider = GetComponent<BoxCollider2D>();
    }
    public Vector2 GetRandomPoint()
    {
        Vector2 center = (Vector2)transform.position + _zoneCollider.offset;
        Vector2 size = _zoneCollider.size;

        float randomX = Random.Range(center.x - size.x / 2f, center.x + size.x / 2f);
        float randomY = Random.Range(center.y - size.y / 2f, center.y + size.y / 2f);

        return new Vector2(randomX, randomY);
    }
}
