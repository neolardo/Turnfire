using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExplosionManager : MonoBehaviour //TODO: pool base class
{
    [SerializeField][Range(2, 10)] private int numExplosions = 5;
    [SerializeField] private Explosion _explosionPrefab;

    private List<Explosion> _unavailableExplosions;
    private List<Explosion> _availableExplosions;

    private void Start()
    {
        _unavailableExplosions = new List<Explosion>();
        _availableExplosions = new List<Explosion>();
        for (int i = 0; i < numExplosions; i++)
        {
            _availableExplosions.Add(InstantiateExplosion());
        }
    }

    private Explosion InstantiateExplosion()
    {
        var p = Instantiate(_explosionPrefab, transform);
        p.ExplosionFinished += OnExplosionFinished;
        return p;
    }

    private void OnExplosionFinished(Explosion ex)
    {
        _unavailableExplosions.Remove(ex);
        _availableExplosions.Add(ex);
    }


    public Explosion GetExplosion()
    {
        Explosion e = null;
        if (_availableExplosions.Any())
        {
            e = _availableExplosions.First();
            _availableExplosions.Remove(e);
            _unavailableExplosions.Add(e);
        }
        else
        {
            e = InstantiateExplosion();
            _unavailableExplosions.Add(e);
        }
        return e;
    }



    public IEnumerable<Explosion> GetExplosions(int numExplosions)
    {
        var explosions = new List<Explosion>();
        for (int i = 0; i < numExplosions; i++)
        {
            explosions.Add(GetExplosion());
        }
        return explosions;
    }
}
