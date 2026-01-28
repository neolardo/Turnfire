using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class LineRendererFragment : MonoBehaviour
{
    [SerializeField] private Material _material;
    [SerializeField] private int _pixelsPerUnit = Constants.PixelsPerUnit;

    private SpriteRenderer _sr;
    private Vector4[] _points = new Vector4[MaxPoints];

    private const int MaxPoints = 128;
    private const float MapMargin = 7;

    private void Start()
    {
        var terrain = FindFirstObjectByType<TerrainManager>();
        var pixelWidth = (int)(terrain.PixelSize.x + MapMargin * 2 * _pixelsPerUnit);
        var pixelHeight = (int)(terrain.PixelSize.y + MapMargin * 2 * _pixelsPerUnit);
        _sr = GetComponent<SpriteRenderer>();
        InitializeSpriteRenderer(pixelWidth, pixelHeight);
    }

    private void InitializeSpriteRenderer(int width, int height)
    {
        _sr.material = _material;
        _material.SetVector("_Size", new Vector2(width, height));
        var tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.Apply();

        _sr.sprite = Sprite.Create(
            tex,
            new Rect(0, 0, width, height),
            new Vector2(0.5f, 0.5f),
            _pixelsPerUnit
        );
    }


    public void DrawLine(IList<Vector2> worldPoints)
    {
        int count = Mathf.Min(worldPoints.Count, MaxPoints);

        for (int i = 0; i < count; i++)
        {
            _points[i] = WorldToPixel(worldPoints[i]);
        }

        _material.SetInt("_PointCount", count);
        _material.SetVectorArray("_Points", _points);
    }

    public void Clear()
    {
        _material.SetInt("_PointCount", 0);
    }

    private Vector2 WorldToPixel(Vector2 worldPos)
    {
        Vector3 local = transform.InverseTransformPoint(worldPos);

        float px = (local.x + _sr.sprite.bounds.extents.x) * _pixelsPerUnit;
        float py = (local.y + _sr.sprite.bounds.extents.y) * _pixelsPerUnit;

        return new Vector2(Mathf.RoundToInt(px), Mathf.RoundToInt(py));
    }

}
