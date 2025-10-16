using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
[RequireComponent(typeof(TilemapCollider2D))]
[RequireComponent(typeof(TilemapRenderer))]
public class DestructibleTilemapTexture : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _pixelsPerUnit = 32f;
    [SerializeField] private Material _tilemapMaterial;

    private Tilemap _tilemap;
    private TilemapRenderer _renderer;
    private TilemapCollider2D _collider;

    [SerializeField]  private Texture2D _baseTexture;
    [SerializeField] private Texture2D _maskTexture;
    private Color[] _maskPixels;
    private int _width, _height;
    private Vector2Int _offset;
    private bool _initialized;

    private static readonly int MaskID = Shader.PropertyToID("_AlphaMask");

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (_initialized) return;
        _initialized = true;

        _tilemap = GetComponent<Tilemap>();
        _renderer = GetComponent<TilemapRenderer>();
        _collider = GetComponent<TilemapCollider2D>();

        if (_tilemapMaterial == null)
        {
            Debug.LogError("Assign a material with 'Custom/TilemapAlphaMask' shader!", this);
            return;
        }

        _renderer.material = _tilemapMaterial;

        // Bake the visible tilemap to a single texture
        BakeTilemapToTexture();
        _renderer.material.mainTexture = _baseTexture;

        // Create an alpha mask the same size
        _maskTexture = new Texture2D(_width, _height, TextureFormat.Alpha8, false)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };
        _maskPixels = new Color[_width * _height];
        for (int i = 0; i < _maskPixels.Length; i++)
            _maskPixels[i] = Color.white;

        _maskTexture.SetPixels(_maskPixels);
        _maskTexture.Apply();

        _renderer.material.SetTexture(MaskID, _maskTexture);
    }

    private void BakeTilemapToTexture()
    {
        var bounds = _tilemap.cellBounds;
        var size = bounds.size;
        _width = Mathf.RoundToInt(size.x * _pixelsPerUnit);
        _height = Mathf.RoundToInt(size.y * _pixelsPerUnit);
        _offset = new Vector2Int(bounds.xMin, bounds.yMin);

        _baseTexture = new Texture2D(_width, _height, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };

        // Paint tiles into texture
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                var tile = _tilemap.GetTile(new Vector3Int(x, y, 0)) as Tile;
                if (tile == null) continue;
                if (tile.sprite == null) continue;

                // Copy sprite pixels into texture
                var sprite = tile.sprite;
                Rect rect = sprite.textureRect;
                Color[] pixels = sprite.texture.GetPixels(
                    (int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);

                int px = Mathf.RoundToInt((x - bounds.xMin) * _pixelsPerUnit);
                int py = Mathf.RoundToInt((y - bounds.yMin) * _pixelsPerUnit);

                for (int sx = 0; sx < rect.width; sx++)
                {
                    for (int sy = 0; sy < rect.height; sy++)
                    {
                        int tx = px + sx;
                        int ty = py + sy;
                        if (tx < 0 || ty < 0 || tx >= _width || ty >= _height) continue;

                        Color c = pixels[sy * (int)rect.width + sx];
                        _baseTexture.SetPixel(tx, ty, c);
                    }
                }
            }
        }

        _baseTexture.Apply();
    }

    public void ApplyExplosion(Vector2 worldPos, float radius)
    {
        if (!_initialized) Initialize();

        Vector3 localPos = transform.InverseTransformPoint(worldPos);
        Vector2 texturePos = new Vector2(
            (localPos.x - _offset.x) * _pixelsPerUnit,
            (localPos.y - _offset.y) * _pixelsPerUnit
        );

        int r = Mathf.RoundToInt(radius * _pixelsPerUnit);
        for (int x = -r; x <= r; x++)
        {
            for (int y = -r; y <= r; y++)
            {
                if (x * x + y * y > r * r)
                    continue;

                int px = Mathf.RoundToInt(texturePos.x + x);
                int py = Mathf.RoundToInt(texturePos.y + y);

                if (px >= 0 && px < _width && py >= 0 && py < _height)
                    _maskPixels[py * _width + px] = Color.clear;
            }
        }

        _maskTexture.SetPixels(_maskPixels);
        _maskTexture.Apply();

        _collider.ProcessTilemapChanges();
    }
}
