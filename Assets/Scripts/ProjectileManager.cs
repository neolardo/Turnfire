using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    [SerializeField] [Range(5, 20)] private int numProjectiles =  10;
    [SerializeField] private Projectile _projectilePrefab;

    private List<Projectile> _unavailableProjectiles;
    private List<Projectile> _availableProjectiles;

    private void Start()
    {
        _unavailableProjectiles = new List<Projectile>();
        _availableProjectiles = new List<Projectile>();
        for (int i = 0; i < numProjectiles; i++)
        {
            _availableProjectiles.Add(InstantiateProjectile());
        }
    }

    private Projectile InstantiateProjectile()
    {
        var p = Instantiate(_projectilePrefab, transform);
        p.Exploded += OnProjectileExploded;
        return p;
    }

    private void OnProjectileExploded(ExplosionInfo ei)
    {
        _unavailableProjectiles.Remove(ei.Source);
        _availableProjectiles.Add(ei.Source);
    }


    public Projectile GetProjectile()
    {
        Projectile p = null;
        if (_availableProjectiles.Any())
        {
            p = _availableProjectiles.First();
            _availableProjectiles.Remove(p);
            _unavailableProjectiles.Add(p);
        }
        else
        {
            p = InstantiateProjectile();
            _unavailableProjectiles.Add(p);
        }
        return p;
    }



    public IEnumerable<Projectile> GetProjectiles(int numProjectiles)
    {
        var projectiles = new List<Projectile>();
        for(int i = 0;i < numProjectiles;i++)
        {
            projectiles.Add(GetProjectile());
        }
        return projectiles;
    }

}
