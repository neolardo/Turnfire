using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class LineRendererCompute : MonoBehaviour
{
    [Header("Texture Size")]
    [SerializeField] private int _pixelsPerUnit = 64;
    private int _textureWidth;
    private int _textureHeight;

    [Header("Line Settings")]
    [SerializeField] private Color _lineColor = Color.red;
    [SerializeField][Range(1, 5)] private int _thickness = 1;
    private Vector2Int[] _points = Array.Empty<Vector2Int>();

    [Header("References")]
    [SerializeField] private ComputeShader _compute;
    [SerializeField] private Material _material; 

    [SerializeField] private RenderTexture _rt;
    private ComputeBuffer _pointsBuffer;
    private SpriteRenderer _sr;
    private int _kernel;
    private const string MainKernel = "CSMain";

    private void Start()
    {
        var terrainRenderer = FindFirstObjectByType<DestructibleTerrainRenderer>();
        _textureWidth = terrainRenderer.Texture.width;
        _textureHeight = terrainRenderer.Texture.height;
        _sr = GetComponent<SpriteRenderer>();
        InitializeSpriteRenderer();

        CreateRT();
        _kernel = _compute.FindKernel(MainKernel);
        Dispatch();
    }

    private void InitializeSpriteRenderer()
    {
        var tex = new Texture2D(_textureWidth, _textureHeight, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.Apply();

        // Replace sprite's texture with editable one
        _sr.sprite = Sprite.Create(
            tex,
            new Rect(0, 0, _textureWidth, _textureHeight),
            new Vector2(0.5f, 0.5f),
            _pixelsPerUnit
        );
    }

    private void OnDisable()
    {
        ReleaseBuffers();
        ReleaseRT();
    }

    public void DrawLine(Vector2[] worldPoints)
    {
        _points = worldPoints.Select(p => WorldToPixel(p)).ToArray();
        Dispatch();
    }

    public void Clear()
    {
        _points = null;
        Dispatch();
    }

    private void CreateRT()
    {
        if (_rt != null &&_rt.width == _textureWidth && _rt.height == _textureHeight)
            return;

        ReleaseRT();

        _rt = new RenderTexture(_textureWidth, _textureHeight, 0, RenderTextureFormat.ARGB32)
        {
            enableRandomWrite = true,
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };

        _rt.Create();

        if (_material != null)
        {
            _material.SetTexture("_RenderTex", _rt);
        }
    }

    private Vector2Int WorldToPixel(Vector2 worldPos)
    {
        Vector3 local = transform.InverseTransformPoint(worldPos);

        float px = (local.x + _sr.sprite.bounds.extents.x) * _pixelsPerUnit;
        float py = (local.y + _sr.sprite.bounds.extents.y) * _pixelsPerUnit;

        return new Vector2Int(Mathf.RoundToInt(px), Mathf.RoundToInt(py));
    }

    private void ReleaseRT()
    {
        if (_rt != null)
        {
            _rt.Release();
            UnityEngine.Object.DestroyImmediate(_rt);
            _rt = null;
        }
    }

    private void ReleaseBuffers()
    {
        if (_pointsBuffer != null)
        {
            _pointsBuffer.Release();
            _pointsBuffer = null;
        }
    }

    private void Dispatch()
    {
        if (_compute == null)
            return;

        CreateRT();

        // Flatten points
        int count = _points != null ? _points.Length : 0;
        int[] flat = new int[count * 2];

        for (int i = 0; i < count; i++)
        {
            flat[i * 2 + 0] = _points[i].x;
            flat[i * 2 + 1] = _points[i].y;
        }

        ReleaseBuffers();
        _pointsBuffer = new ComputeBuffer(Mathf.Max(1, flat.Length), sizeof(int));
        _pointsBuffer.SetData(flat.Length > 0 ? flat : new int[] { 0 });

        // Set compute shader parameters
        _compute.SetTexture(_kernel, "Result", _rt);
        _compute.SetBuffer(_kernel, "Points", _pointsBuffer);
        _compute.SetInt("pointCount", count);
        _compute.SetInts("texSize", _textureWidth, _textureHeight);
        _compute.SetVector("lineColor", new Vector4(_lineColor.r, _lineColor.g, _lineColor.b, _lineColor.a));
        _compute.SetInt("thickness", _thickness);

        // Dispatch
        int tx = Mathf.CeilToInt(_textureWidth / 8f);
        int ty = Mathf.CeilToInt(_textureHeight / 8f);
        _compute.Dispatch(_kernel, tx, ty, 1);
    }
}
