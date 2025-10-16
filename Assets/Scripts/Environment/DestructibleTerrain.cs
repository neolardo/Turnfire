using UnityEngine;
using UnityEngine.Tilemaps;

public class DestructibleTerrain : MonoBehaviour
{
    [SerializeField] private int _pixelsPerUnit = 64;
    [SerializeField] private int _pixelsPerTile = 32;
    [SerializeField] private ExplosionHole _explosionHolePrefab;
    [SerializeField] private Transform _explosionHoleContainer;
    [SerializeField] private Vector2 _terrainOffset;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    private Texture2D _sourceTexture;
    private int _width, _height;
    private Vector2 _tilemapOffset;

    private void Start()
    {
        BakeTilemapToTexture();
    }

    public void InitializeFromTexture(Texture2D texture)
    {        
        _sourceTexture = texture;
        _spriteRenderer.transform.position = _terrainOffset;
        _spriteRenderer.sprite = Sprite.Create(_sourceTexture, new Rect(Vector2.zero, new Vector2(_sourceTexture.width, _sourceTexture.height)), Vector2.zero, _pixelsPerUnit, 0, SpriteMeshType.Tight);
        _width = _sourceTexture.width;
        _height = _sourceTexture.height;
    }

    #region Tilemap To Texture

    private void BakeTilemapToTexture()
    {
        var tilemap = GetComponent<Tilemap>();
        var tilemapRenderer = GetComponent<TilemapRenderer>();

        Texture2D texture = BakeTilemapToTexture(tilemap);

        var bounds = tilemap.cellBounds;
        Vector3Int centerCell = new Vector3Int((bounds.xMin + bounds.xMax) / 2, (bounds.yMin + bounds.yMax) / 2, 0);
        
        _tilemapOffset = tilemap.CellToWorld(centerCell) + Vector3.up * (tilemap.layoutGrid.cellSize.y / 2);

        tilemapRenderer.enabled = false;

        InitializeFromTexture(texture);
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

    public void ApplyExplosion(Vector2 worldPos, float radius)
    {
        Vector2 local = transform.InverseTransformPoint(worldPos);
        local -= _tilemapOffset;
        float ppu = _pixelsPerUnit;

        int px = Mathf.RoundToInt(local.x * ppu + _width / 2f);
        int py = Mathf.RoundToInt(local.y * ppu + _height / 2f);
        int r = Mathf.RoundToInt(radius * ppu);

        for (int y = -r; y <= r; y++)
            for (int x = -r; x <= r; x++)
            {
                if (x * x + y * y > r * r) continue;

                int tx = px + x;
                int ty = py + y;
                if (tx < 0 || ty < 0 || tx >= _width || ty >= _height) continue;

                _sourceTexture.SetPixel(tx, ty, Color.clear);
            }

        _sourceTexture.Apply();

        var hole = Instantiate(_explosionHolePrefab, _explosionHoleContainer);
        hole.Initialize(worldPos, radius);

    }


}
