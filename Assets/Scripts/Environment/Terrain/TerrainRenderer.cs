using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TerrainRenderer : MonoBehaviour
{
    private SpriteRenderer _renderer;

    public void Initialize(TerrainTexture terrainTexture, Vector2 rendererOffset, int pixelsPerUnit)
    {
        _renderer = GetComponent<SpriteRenderer>();
        _renderer.transform.position = rendererOffset;

        _renderer.sprite = Sprite.Create(
            terrainTexture.Texture,
            new Rect(Vector2.zero, new Vector2(terrainTexture.Texture.width, terrainTexture.Texture.height)),
            Vector2.zero,
            pixelsPerUnit,
            0,
            SpriteMeshType.Tight
        );
    }
}
