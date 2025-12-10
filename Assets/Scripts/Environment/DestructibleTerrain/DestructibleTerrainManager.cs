using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DestructibleTerrainManager : MonoBehaviour
{
    [Header("Terrain")]
    [SerializeField] private DestructibleTerrainRenderer _renderer;
    [SerializeField] private DestructibleTerrainCollider _collider;
    [Header("Tilemap")]
    [SerializeField] private TilemapCollider2D _tilemapCollider;
    [SerializeField] private TilemapRenderer _tilemapRenderer;
    [SerializeField] private Tilemap _tilemap;
    [Header("Explosion Hole")]
    [SerializeField] private ExplosionHolePool _explosionHolePool;
    [SerializeField] private Transform _initialExplosionHoleContainer;
    [SerializeField] private int _explosionHoleThresholdForColliderRebuild;

    public Vector2 Size => _renderer.Size;
    public Vector2 PixelSize => _renderer.PixelSize;

    private Transform _explosionHoleContainer;

    private bool _firstRebuildDone;

    private List<ExplosionHole> _newHoles;
    private List<ExplosionHole> _removableHoles;

    public event Action<Vector2, float> TerrainModifiedByExplosion;

    private void Awake()
    {
        _explosionHoleContainer = _initialExplosionHoleContainer;
        _newHoles = new List<ExplosionHole>();
        _removableHoles = new List<ExplosionHole>();
        _renderer.InitializeFromTilemap(_tilemap, _tilemapRenderer);
        _collider.RebuildFinished += OnColliderRebuildFinished;
        SafeObjectPlacer.SetDestructibleTerrain(this);
    }

    private void Start()
    {
        InitiateColliderRebuild();
    }

    #region Explosion

    public void ApplyExplosion(Vector2 position, float radius)
    {
        _renderer.ApplyExplosion(position, radius);
        AddNewExplosionHole(position, radius);
        TerrainModifiedByExplosion?.Invoke(position, radius);
        if (_newHoles.Count >= _explosionHoleThresholdForColliderRebuild)
        {
            InitiateColliderRebuild();
        }
    }


    private void AddNewExplosionHole(Vector2 position, float radius)
    {
        var hole = _explosionHolePool.Get();
        hole.transform.SetParent(_explosionHoleContainer, true);
        hole.Initialize(position, radius);
        _newHoles.Add(hole);
    }

    #endregion

    #region Collider Rebuild

    private void InitiateColliderRebuild()
    {
        if (_collider.RebuildInProgress)
        {
            return;
        }

        _removableHoles.AddRange(_newHoles);
        _newHoles.Clear();
        _collider.InitiateRebuild(_renderer.Texture, _renderer.CenteredPivotOffset);
        Debug.Log($"{nameof(DestructibleTerrainCollider)} rebuild started.");
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

        Debug.Log($"{nameof(DestructibleTerrainCollider)} rebuild finished.");
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

    #region Overlap and Bound Checks

    public bool OverlapCircle(Vector2 worldPos, float radius)
    {
        return _renderer.OverlapCircle(worldPos, radius);
    }

    public bool OverlapPoint(Vector2 worldPos)
    {
        return _renderer.OverlapPoint(worldPos);
    }

    public bool IsPointInsideBounds(Vector2 worldPos)
    {
        return _renderer.IsPointInsideBounds(worldPos);
    }

    #endregion

    #region Normal Calculation

    public Vector2 GetNearestNormalAtPoint(Vector2 worldPos)
    {
        return _renderer.GetNearestNormalAtPoint(worldPos);
    }

    #endregion

    #region Standing Point

    public bool TryFindNearestStandingPoint(Vector2Int pixelCoordinates, int searchRadius, int standingPointId, out StandingPoint result)
    {
        return _renderer.TryFindNearestStandingPoint(pixelCoordinates, searchRadius, standingPointId, out result);
    }

    public bool TryFindNearestStandingPoint(Vector2 worldPos, int searchRadius, int standingPointId, out StandingPoint result)
    {
        return _renderer.TryFindNearestStandingPoint(worldPos, searchRadius, standingPointId, out result);
    }

    public bool IsCornerPoint(Vector2Int pixelCoordinates)
    {
        return _renderer.IsCornerPoint(pixelCoordinates);
    }

    #endregion

}
