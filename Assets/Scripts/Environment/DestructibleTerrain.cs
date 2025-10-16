using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CompositeCollider2D))]
public class DestructibleTerrain: MonoBehaviour
{
    private Texture2D _texture;
    private SpriteRenderer _renderer;
    private PolygonCollider2D[] _polygonColliders;
    private CompositeCollider2D _compositeCollider;

    public void Initialize(Sprite terrainSprite)
    {
        _renderer = GetComponent<SpriteRenderer>();
        _compositeCollider = GetComponent<CompositeCollider2D>();
        _polygonColliders = new PolygonCollider2D[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            _polygonColliders[i] = transform.GetChild(i).GetComponent<PolygonCollider2D>();
        }

        _renderer.sprite = terrainSprite;
        _texture = terrainSprite.texture;
    }

    public void ApplyExplosion(Vector2 worldPos, float radius)
    {
        Vector2 localPos = transform.InverseTransformPoint(worldPos);
        Sprite sprite = _renderer.sprite;

        float ppu = sprite.pixelsPerUnit;
        Vector2 pivot = sprite.pivot;

        // Sprite size in world units (before scaling)
        float spriteWorldWidth = sprite.rect.width / ppu;
        float spriteWorldHeight = sprite.rect.height / ppu;

        // Texture coordinate scale (handles scaling)
        float unitsToPixelsX = _texture.width / (spriteWorldWidth * transform.localScale.x);
        float unitsToPixelsY = _texture.height / (spriteWorldHeight * transform.localScale.y);

        // Convert local position to pixel coordinates
        int pxCenter = Mathf.RoundToInt(localPos.x * unitsToPixelsX + pivot.x);
        int pyCenter = Mathf.RoundToInt(localPos.y * unitsToPixelsY + pivot.y);

        // World radius → pixels
        int rX = Mathf.RoundToInt(radius * unitsToPixelsX);
        int rY = Mathf.RoundToInt(radius * unitsToPixelsY);

        // Iterate ellipse for scaled explosions
        for (int x = -rX; x <= rX; x++)
        {
            for (int y = -rY; y <= rY; y++)
            {
                float normX = (float)x / rX;
                float normY = (float)y / rY;

                if (normX * normX + normY * normY <= 1f)
                {
                    int px = pxCenter + x;
                    int py = pyCenter + y;

                    if (px >= 0 && px < _texture.width && py >= 0 && py < _texture.height)
                    {
                        _texture.SetPixel(px, py, Color.clear);
                    }
                }
            }
        }
        _texture.Apply();
        RebuildCollider();
    }

    private void RebuildCollider()
    {
        //for (int i = 0; i < _polygonColliders.Length; i++)
        //{
        //    var col = _polygonColliders[i];
        //    Destroy(col);
        //    _polygonColliders[i].gameObject.AddComponent<PolygonCollider2D>(); 
        //}
        //TODO
      
    }
}