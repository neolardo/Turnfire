using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(SpriteRenderer))]
public class DestructibleTerrainRenderer : MonoBehaviour
{
    [SerializeField] private int _pixelsPerTile = 32;
    [SerializeField] private int _pixelsPerUnit = 64;

    private SpriteRenderer _renderer;
    private int _width, _height;
    public Texture2D Texture { get; private set; }

    public Vector2 CenteredPivotOffset { get; private set; }

    private Vector2 _textureOffset;

    #region Initialize

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    public void InitializeFromTilemap(Tilemap tilemap, TilemapRenderer tilemapRenderer)
    {
        Texture = BakeTilemapToTexture(tilemap);
        _width = Texture.width;
        _height = Texture.height;
        CenteredPivotOffset = (tilemap.CellToWorld(tilemap.cellBounds.min) + tilemap.CellToWorld(tilemap.cellBounds.max)) / 2;
        _textureOffset = tilemap.CellToWorld(tilemap.cellBounds.min);
        _renderer.transform.position = _textureOffset;
        _renderer.sprite = Sprite.Create(Texture, new Rect(Vector2.zero, new Vector2(Texture.width, Texture.height)), Vector2.zero, _pixelsPerUnit, 0, SpriteMeshType.Tight);
        tilemapRenderer.enabled = false;
    }

    private Texture2D GetReadableTexture(Sprite sprite)
    {
        var src = sprite.texture;

        RenderTexture tmp = RenderTexture.GetTemporary(
            src.width, src.height, 0,
            RenderTextureFormat.Default, RenderTextureReadWrite.sRGB
        );

        Graphics.Blit(src, tmp);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = tmp;

        Texture2D readableTex = new Texture2D(src.width, src.height, TextureFormat.RGBA32, false, false);
        readableTex.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
        readableTex.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(tmp);

        return readableTex;
    }

    private Texture2D BakeTilemapToTexture(Tilemap tilemap)
    {
        var bounds = tilemap.cellBounds;

        var texture = new Texture2D(bounds.size.x * _pixelsPerTile, bounds.size.y * _pixelsPerTile, TextureFormat.RGBA32, false);
        var clear = new Color[texture.width * texture.height];
        texture.SetPixels(clear);

        foreach (var pos in bounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(pos);
            if (tile == null) continue;

            Sprite sprite = tilemap.GetSprite(pos);
            if (sprite == null) continue;

            var rect = sprite.textureRect;
            Texture2D readableTex = GetReadableTexture(sprite);
            var spritePixels = readableTex.GetPixels(
                (int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height
            );

            int px = (pos.x - bounds.xMin) * _pixelsPerTile;
            int py = (pos.y - bounds.yMin) * _pixelsPerTile;
            texture.SetPixels(px, py, (int)rect.width, (int)rect.height, spritePixels);
        }

        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();
        return texture;
    }

    #endregion

    #region Explosion

    public void ApplyExplosion(Vector2 worldPos, float radius)
    {
        Vector2 local = worldPos - CenteredPivotOffset;

        int px = Mathf.RoundToInt(local.x * _pixelsPerUnit + _width / 2f);
        int py = Mathf.RoundToInt(local.y * _pixelsPerUnit + _height / 2f);
        int r = Mathf.RoundToInt(radius * _pixelsPerUnit);

        for (int y = -r; y <= r; y++)
            for (int x = -r; x <= r; x++)
            {
                if (x * x + y * y > r * r) continue;

                int tx = px + x;
                int ty = py + y;
                if (tx < 0 || ty < 0 || tx >= _width || ty >= _height) continue;

                Texture.SetPixel(tx, ty, Color.clear);
            }

        Texture.Apply();
    } 

    #endregion
}
