using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(PolygonCollider2D))]
public class DestructibleTerrain: MonoBehaviour
{
    private Texture2D _texture;
    private SpriteRenderer _renderer;
    private PolygonCollider2D _collider;

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<PolygonCollider2D>();
        // Make the texture writable

        // Clone the texture so we can modify it
        Texture2D source = _renderer.sprite.texture;
        _texture = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);
        _texture.filterMode = source.filterMode;
        _texture.wrapMode = source.wrapMode;
        _texture.SetPixels(source.GetPixels());
        _texture.Apply();

        // Replace sprite with runtime sprite using our texture
        _renderer.sprite = Sprite.Create(
            _texture,
            _renderer.sprite.rect,
            new Vector2(0.5f, 0.5f),
            _renderer.sprite.pixelsPerUnit
        );
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
        Destroy(_collider);
        _collider = gameObject.AddComponent<PolygonCollider2D>();
    }
}