using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[RequireComponent(typeof(CompositeCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class DestructibleTerrainCollider : MonoBehaviour
{
    [SerializeField] private DestructibleIslandCollider _islandColliderPrefab;
    [SerializeField] private int _pixelsPerUnit = 64;

    private PixelMask _terrainPixelMask;
    private List<DestructibleIslandCollider> _islandColliders;

    private int Width => _terrainPixelMask.Width; 
    private int Height => _terrainPixelMask.Height;

    public event Action RebuildFinished;
    public bool RebuildInProgress { get; private set; }

    private void Awake()
    {
        _islandColliders = new List<DestructibleIslandCollider>();
    }

    public void InitiateRebuild(Texture2D texture, Vector2 offset)
    {
        RebuildFromTexture(texture, offset);
    }

    private void RebuildFromTexture(Texture2D texture, Vector2 offset)
    {
        RebuildInProgress = true;
        transform.position = offset;
        _terrainPixelMask = new PixelMask(texture.GetPixels(), texture.width, texture.height);  
        BuildIslands();
        RebuildInProgress = false;
        RebuildFinished?.Invoke();
    }


    #region Islands

    private void BuildIslands()
    {
        ClearIslands();

        bool[,] visited = new bool[Width, Height];
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (_terrainPixelMask[x, y] && !visited[x, y])
                {
                    var pixels = FloodFill(x, y, visited);
                    var islandPixelMask = new PixelMask(pixels);
                    var island = CreateIslandCollider(islandPixelMask);
                    _islandColliders.Add(island);
                }
            }
        }
    }

    private void ClearIslands()
    {
        foreach (var isl in _islandColliders)
        {
            if (isl != null)
            {
                Destroy(isl.gameObject);
            }
        }
        _islandColliders.Clear();
    }

    private List<Vector2Int> FloodFill(int sx, int sy, bool[,] visited)
    {
        Queue<Vector2Int> queue = new();
        List<Vector2Int> pixels = new();

        queue.Enqueue(new Vector2Int(sx, sy));
        visited[sx, sy] = true;

        while (queue.Count > 0)
        {
            var p = queue.Dequeue();
            pixels.Add(p);

            foreach (var d in new Vector2Int[] { new(1, 0), new(-1, 0), new(0, 1), new(0, -1) })
            {
                int nx = p.x + d.x, ny = p.y + d.y;
                if (nx < 0 || ny < 0 || nx >= Width || ny >= Height) continue;
                if (!_terrainPixelMask[nx, ny] || visited[nx, ny]) continue;

                visited[nx, ny] = true;
                queue.Enqueue(new Vector2Int(nx, ny));
            }
        }

        return pixels;
    }


    public DestructibleIslandCollider CreateIslandCollider(PixelMask islandPixelMask)
    {
        var islandCenter = islandPixelMask.Rect.center;
        var terrainCenter = _terrainPixelMask.Rect.center;
        var isl = Instantiate(_islandColliderPrefab);
        isl.name = $"Island_{_islandColliders.Count}";
        isl.transform.SetParent(transform, false);
        isl.transform.localPosition = (islandCenter - terrainCenter) / _pixelsPerUnit;
        isl.RebuildColliderFromPixelMask(islandPixelMask, _pixelsPerUnit);
        _islandColliders.Add(isl);
        return isl;
    }

    #endregion

}