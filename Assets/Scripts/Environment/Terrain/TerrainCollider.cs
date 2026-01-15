using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CompositeCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class TerrainCollider : MonoBehaviour
{
    [SerializeField] private IslandCollider _islandColliderPrefab;

    private PixelMask _terrainPixelMask;
    private List<IslandCollider> _islandColliders;
    private int _pixelsPerUnit;
    private int Width => _terrainPixelMask.Width; 
    private int Height => _terrainPixelMask.Height;

    public event Action RebuildFinished;
    public bool RebuildInProgress { get; private set; }

    private static readonly Vector2Int[] FloodFillDirections = new Vector2Int[] { new (1, 0), new (-1, 0), new (0, 1), new (0, -1) };

    private void Awake()
    {
        _islandColliders = new List<IslandCollider>();
    }

    public void Initialize(int pixelsPerUnit)
    {
        _pixelsPerUnit = pixelsPerUnit;
    }

    public void InitiateRebuild(Texture2D texture, Vector2 offset)
    {
        if (RebuildInProgress)
        {
            return;
        }

        StartCoroutine(RebuildFromTextureAsync(texture, offset));
    }

    private IEnumerator RebuildFromTextureAsync(Texture2D texture, Vector2 offset)
    {
        RebuildInProgress = true;
        transform.position = offset;
        yield return PixelMask.CreateAsync(texture.GetPixels(), texture.width, texture.height, pm => _terrainPixelMask = pm);  
        yield return BuildIslandsAsync();
        RebuildInProgress = false;
        RebuildFinished?.Invoke();
    }

    #region Islands

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

    private IEnumerator BuildIslandsAsync()
    {
        var newIslands = new List<IslandCollider>();

        bool[,] visited = new bool[Width, Height];
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (_terrainPixelMask[x, y] && !visited[x, y])
                {
                    List<Vector2Int> pixels = null;
                    yield return FloodFillAsync(x, y, visited, p => pixels = p );
                    PixelMask islandPixelMask = null;
                    yield return PixelMask.CreateAsync(pixels, pm => islandPixelMask = pm);
                    IslandCollider island = null;
                    yield return CreateIslandColliderAsync(islandPixelMask, newIslands.Count, i=> island = i);
                    newIslands.Add(island);
                }
            }
        }
        ClearIslands();
        _islandColliders.AddRange(newIslands);
    }


    private IEnumerator FloodFillAsync(int sx, int sy, bool[,] visited, Action<List<Vector2Int>> onDone, int yieldInterval = 20000)
    {
        Queue<Vector2Int> queue = new();
        List<Vector2Int> pixels = new();

        queue.Enqueue(new Vector2Int(sx, sy));
        visited[sx, sy] = true;

        int ind = 0;
        while (queue.Count > 0)
        {
            var p = queue.Dequeue();
            pixels.Add(p);

            foreach (var d in FloodFillDirections)
            {
                int nx = p.x + d.x, ny = p.y + d.y;
                if (nx < 0 || ny < 0 || nx >= Width || ny >= Height) continue;
                if (!_terrainPixelMask[nx, ny] || visited[nx, ny]) continue;

                visited[nx, ny] = true;
                queue.Enqueue(new Vector2Int(nx, ny));
            }
            ind++;
            if (ind % yieldInterval == 0)
            {
                yield return null;
            }
        }

        onDone?.Invoke(pixels);
    }


    public IEnumerator CreateIslandColliderAsync(PixelMask islandPixelMask, int index, Action<IslandCollider> onDone)
    {
        var islandCenter = islandPixelMask.Rect.center;
        var terrainCenter = _terrainPixelMask.Rect.center;
        var isl = Instantiate(_islandColliderPrefab);
        isl.name = "Island" + index;
        isl.transform.SetParent(transform, false);
        isl.transform.localPosition = (islandCenter - terrainCenter) / _pixelsPerUnit;
        yield return isl.RebuildColliderFromPixelMaskAsync(islandPixelMask, _pixelsPerUnit);
        onDone?.Invoke(isl);
    }

    #endregion

}