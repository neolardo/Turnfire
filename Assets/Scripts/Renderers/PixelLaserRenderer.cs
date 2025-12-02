using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PixelLaserRenderer : MonoBehaviour
{
    [Header("Laser Appearance")]
    public Color _laserColor = Color.red;
    public float _intensity = 1.0f;
    public float _fadeDuration = 3f;
    public float _maxAlpha = 1f;
    public int _pixelsPerUnit = 64;

    private SpriteRenderer _sr;
    private Texture2D _tex;
    private Color32[] _buffer;

    private int _texWidth;
    private int _texHeight;

    private float _timer;
    private bool _fading;

   private void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
        InitializeTexture();
    }

    private void InitializeTexture()
    {

        var terrainRenderer = FindFirstObjectByType<DestructibleTerrainRenderer>();
        _texWidth = terrainRenderer.Texture.width;
        _texHeight = terrainRenderer.Texture.height;

        _tex = new Texture2D(_texWidth, _texHeight, TextureFormat.RGBA32, false);
        _tex.filterMode = FilterMode.Point;
        _tex.wrapMode = TextureWrapMode.Clamp;

        _buffer = new Color32[_texWidth * _texHeight];
        Clear();

        _tex.SetPixels32(_buffer);
        _tex.Apply();

        // Replace sprite's texture with editable one
        _sr.sprite = Sprite.Create(
            _tex,
            new Rect(0, 0, _texWidth, _texHeight),
            new Vector2(0.5f, 0.5f),
            _pixelsPerUnit
        );
    }

    /// <summary>
    /// Draws a laser path using world-space reflection points.
    /// Call once per shot.
    /// </summary>
    public void DrawLaser(Vector2[] worldPoints)
    {
        _fading = true;
        _timer = 0f;

        for (int i = 0; i < worldPoints.Length - 1; i++)
        {
            DrawLine(
                WorldToPixel(worldPoints[i]),
                WorldToPixel(worldPoints[i + 1])
            );
        }

        _tex.SetPixels32(_buffer);
        _tex.Apply();
    }

    private Vector2Int WorldToPixel(Vector2 worldPos)
    {
        Vector3 local = transform.InverseTransformPoint(worldPos);

        // Sprite uses pivot center + extents
        float px = (local.x + _sr.sprite.bounds.extents.x)
                    * _pixelsPerUnit;
        float py = (local.y + _sr.sprite.bounds.extents.y)
                    * _pixelsPerUnit;

        return new Vector2Int(Mathf.RoundToInt(px), Mathf.RoundToInt(py));
    }

    private void Update()
    {
        if (!_fading)
            return;

        _timer += Time.deltaTime;
        float t = _timer / _fadeDuration;

        if (t >= 1f)
        {
            Clear();
            _tex.SetPixels32(_buffer);
            _tex.Apply();
            _fading = false;
            return;
        }

        float fade = 1f - t;

        for (int i = 0; i < _buffer.Length; i++)
        {
            if (_buffer[i].a == 0) continue;
            Color32 c = _buffer[i];

            c.r = (byte)(c.r * fade);
            c.g = (byte)(c.g * fade);
            c.b = (byte)(c.b * fade);
            c.a = (byte)(c.a * fade);

            _buffer[i] = c;
        }

        _tex.SetPixels32(_buffer);
        _tex.Apply();
    }

    private void DrawLine(Vector2Int a, Vector2Int b)
    {
        int x0 = a.x, y0 = a.y; //TODO: shader code instead?
        int x1 = b.x, y1 = b.y;

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            Plot(x0, y0);
            if (x0 == x1 && y0 == y1) break;

            int e2 = 2 * err;
            if (e2 > -dy) { err -= dy; x0 += sx; }
            if (e2 < dx) { err += dx; y0 += sy; }
        }
    }

    private void Plot(int x, int y)
    {
        if (x < 0 || x >= _texWidth || y < 0 || y >= _texHeight)
            return;

        int idx = y * _texWidth + x;

        Color c = _buffer[idx];
        c += (Color)_laserColor * _intensity;
        c.a = _maxAlpha;

        _buffer[idx] = c;
    }

    private void Clear()
    {
        for (int i = 0; i < _buffer.Length; i++)
            _buffer[i] = new Color32(0, 0, 0, 0);
    }
}
