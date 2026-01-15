using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainManager : MonoBehaviour
{
    [Header("Terrain")]
    [SerializeField] private TerrainRenderer _renderer;
    [SerializeField] private TerrainCollider _collider;
    [SerializeField] private int _pixelsPerUnit = Constants.PixelsPerUnit;
    [SerializeField] private int _pixelsPerTile = Constants.PixelsPerTile;

    [Header("Tilemap")]
    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private TilemapRenderer _tilemapRenderer;
    [SerializeField] private TilemapCollider2D _tilemapCollider;

    [Header("Explosion Hole")]
    [SerializeField] private ExplosionHolePool _explosionHolePool;
    [SerializeField] private Transform _initialExplosionHoleContainer;
    [SerializeField] private int _explosionHoleThresholdForColliderRebuild;

    private TerrainTexture _texture;
    private TerrainPhysics _physics;
    private ITerrainExplosionRegistry _explosionRegistry;

    private Transform _explosionHoleContainer;
    private bool _firstRebuildDone;

    private List<ExplosionHole> _newHoles = new();
    private List<ExplosionHole> _removableHoles = new();

    public event Action<Vector2, float> TerrainModifiedByExplosion;

    public Vector2 Size => new Vector2(_texture.Texture.width / (float)_texture.PixelsPerUnit,
                                       _texture.Texture.height / (float)_texture.PixelsPerUnit);
    public Vector2 PixelSize => new Vector2(_texture.Texture.width, _texture.Texture.height);

    private void Awake()
    {
        _explosionHoleContainer = _initialExplosionHoleContainer;

        _texture = TilemapBaker.Bake(_tilemap, _pixelsPerTile, _pixelsPerUnit);
        _physics = new TerrainPhysics(_texture);

        _collider.Initialize(_pixelsPerUnit);
        var rendererOffset = (Vector2)_tilemap.CellToWorld(_tilemap.cellBounds.min);
        _renderer.Initialize(_texture, rendererOffset, _pixelsPerUnit);

        _collider.RebuildFinished += OnColliderRebuildFinished;
        SafeObjectPlacer.SetDestructibleTerrain(this);

        _tilemapRenderer.enabled = false;
    }

    private void OnDestroy()
    {
        if(_explosionRegistry != null)
        {
            _explosionRegistry.ExplosionRegistered -= ApplyExplosion;
        }
        if(_collider != null)
        {
            _collider.RebuildFinished -= OnColliderRebuildFinished;
        }
    }

    public void SetExplosionRegistry(ITerrainExplosionRegistry explosionRegistry)
    {
        _explosionRegistry = explosionRegistry;
        _explosionRegistry.ExplosionRegistered += ApplyExplosion;
    }

    private void Start()
    {
        InitiateColliderRebuild();
    }

    #region Explosion

    public void RegisterExplosion(Vector2 worldPosition, float radius)
    {
        _explosionRegistry.RegisterExplosion(worldPosition, radius);
    }

    private void ApplyExplosion(Vector2 worldPosition, float radius)
    {
        Vector2 local = _texture.WorldToLocal(worldPosition);
        _texture.ClearCircle(local, radius);

        // Spawn visual holes
        var hole = _explosionHolePool.Get();
        hole.transform.SetParent(_explosionHoleContainer, true);
        hole.Place(worldPosition, radius);
        _newHoles.Add(hole);

        TerrainModifiedByExplosion?.Invoke(worldPosition, radius);

        if (_newHoles.Count >= _explosionHoleThresholdForColliderRebuild)
        {
            InitiateColliderRebuild();
        }
    }

    #endregion

    #region Collider Rebuild

    private void InitiateColliderRebuild()
    {
        if (_collider.RebuildInProgress)
            return;

        _removableHoles.AddRange(_newHoles);
        _newHoles.Clear();
        _collider.InitiateRebuild(_texture.Texture, _texture.CenteredPivotOffset);
        Debug.Log($"{nameof(TerrainCollider)} rebuild started.");
    }

    private void OnColliderRebuildFinished()
    {
        foreach (var hole in _removableHoles)
        {
            _explosionHolePool.Release(hole);
        }
        _removableHoles.Clear();

        if (!_firstRebuildDone)
        {
            OnFirstRebuildDone();
        }

        Debug.Log($"{nameof(TerrainCollider)} rebuild finished.");
    }

    private void OnFirstRebuildDone()
    {
        _explosionHoleContainer = _collider.transform;
        foreach (var hole in _newHoles)
        {
            hole.transform.SetParent(_explosionHoleContainer, true);
        }
        _tilemapCollider.enabled = false;
        _tilemapCollider.gameObject.SetActive(false);
        _firstRebuildDone = true;
    }

    #endregion

    #region Overlap / Bounds

    public bool OverlapCircle(Vector2 worldPos, float radius)
    {
        Vector2 local = _texture.WorldToLocal(worldPos);
        return _physics.OverlapCircle(local, radius);
    }

    public bool OverlapPoint(Vector2 worldPos)
    {
        Vector2 local = _texture.WorldToLocal(worldPos);
        return _physics.OverlapPoint(local);
    }

    public bool IsPointInsideBounds(Vector2 worldPos)
    {
        Vector2 local = _texture.WorldToLocal(worldPos);
        Vector2Int pixel = _texture.LocalPointToPixel(local);
        return !_texture.IsPixelOutOfBounds(pixel);
    }

    #endregion

    #region Normal Calculation

    public Vector2 GetNearestNormalAtPoint(Vector2 worldPos)
    {
        Vector2 local = _texture.WorldToLocal(worldPos);
        return _physics.GetNearestNormal(local);
    }

    #endregion

    #region Standing Points

    public bool TryFindNearestStandingPoint(Vector2 worldPos, int searchRadius, int standingPointId, out StandingPoint result)
    {
        Vector2 local = _texture.WorldToLocal(worldPos);
        var pixel = _texture.LocalPointToPixel(local);
        return _physics.TryFindNearestStandingPoint(pixel, searchRadius, standingPointId, out result);
    }

    public bool TryFindNearestStandingPoint(Vector2Int pixel, int searchRadius, int standingPointId, out StandingPoint result)
    {
        return _physics.TryFindNearestStandingPoint(pixel, searchRadius, standingPointId, out result);
    }

    public bool IsCornerPoint(Vector2Int pixelCoordinates)
    {
        Vector2 local = _texture.PixelToLocalPoint(pixelCoordinates);
        return _physics.IsCorner(local);
    }

    #endregion
}
