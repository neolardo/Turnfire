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

    private Transform _explosionHoleContainer;

    private bool _firstRebuildDone;

    private List<ExplosionHole> _newHoles;
    private List<ExplosionHole> _removableHoles;

    private void Awake()
    {
        _explosionHoleContainer = _initialExplosionHoleContainer;
        _newHoles = new List<ExplosionHole>();
        _removableHoles = new List<ExplosionHole>();
        _renderer.InitializeFromTilemap(_tilemap, _tilemapRenderer);
        _collider.RebuildFinished += OnColliderRebuildFinished;
    }

    public void ApplyExplosion(Vector2 position, float radius)
    {
        _renderer.ApplyExplosion(position, radius);
        AddNewExplosionHole(position, radius);
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

    private void InitiateColliderRebuild()
    {
        if(_collider.RebuildInProgress)
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
        foreach(var hole in _removableHoles)
        {
            _explosionHolePool.Release(hole);
        }
        _removableHoles.Clear();

        if(!_firstRebuildDone)
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


}
