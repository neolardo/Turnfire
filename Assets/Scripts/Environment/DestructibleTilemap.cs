using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
[RequireComponent(typeof(TilemapRenderer))]
[RequireComponent(typeof(TilemapCollider2D))]
public class DestructibleTilemap : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField] private float _pixelsPerUnit = 64f;
    [SerializeField] private Material _tilemapMaterial;

    private Tilemap _tilemap;
    private TilemapRenderer _renderer;
    private TilemapCollider2D _collider;
    [SerializeField]  private Texture2D _maskTexture;
    private Color[] _maskPixels;
    private int _width, _height;
    private bool _initialized;

    private static readonly int MaskID = Shader.PropertyToID("_AlphaMask");

    private bool _ready;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (_ready)
            return;
        _ready = true;

        _tilemap = GetComponent<Tilemap>();
        _renderer = GetComponent<TilemapRenderer>();
        _collider = GetComponent<TilemapCollider2D>();

        if (_tilemapMaterial == null)
        {
            Debug.LogError($"{name}: no material assigned for destructible tilemap.");
            return;
        }

        _renderer.material = _tilemapMaterial;
        CreateMaskTexture();
        _renderer.material.SetTexture(MaskID, _maskTexture);
    }


    private void CreateMaskTexture()
    {
        var bounds = _tilemap.cellBounds;
        _width = Mathf.CeilToInt(bounds.size.x * _pixelsPerUnit);
        _height = Mathf.CeilToInt(bounds.size.y * _pixelsPerUnit);

        _maskTexture = new Texture2D(_width, _height, TextureFormat.Alpha8, false)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };

        _maskPixels = new Color[_width * _height];
        for (int i = 0; i < _maskPixels.Length; i++)
            _maskPixels[i] = Color.white; // start fully solid

        _maskTexture.SetPixels(_maskPixels);
        _maskTexture.Apply();
    }

    public void ApplyExplosion(Vector2 worldPos, float radius)
    {
        if (!_ready) Initialize();

        Vector3 local = transform.InverseTransformPoint(worldPos);
        var bounds = _tilemap.cellBounds;
        Vector2 offset = new Vector2(bounds.xMin, bounds.yMin);
        Vector2 pixelPos = (new Vector2(local.x, local.y) - offset) * _pixelsPerUnit;

        int r = Mathf.RoundToInt(radius * _pixelsPerUnit);
        for (int x = -r; x <= r; x++)
        {
            for (int y = -r; y <= r; y++)
            {
                if (x * x + y * y > r * r) continue;
                int px = Mathf.RoundToInt(pixelPos.x + x);
                int py = Mathf.RoundToInt(pixelPos.y + y);
                if (px < 0 || px >= _width || py < 0 || py >= _height) continue;
                _maskPixels[py * _width + px] = Color.clear;
            }
        }

        _maskTexture.SetPixels(_maskPixels);
        _maskTexture.Apply();
        UpdateCollider(worldPos, radius);
    }
    private void UpdateCollider(Vector2 worldPos, float radius)
    {
        // Simplified solution: just refresh collider entirely
        // (for small tilemaps this is fine; large maps may need chunk updates)
        _collider.ProcessTilemapChanges();
    }
}