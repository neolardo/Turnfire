using System.Collections;
using System.Linq;
using UnityEngine;

public class Weapon : Item
{
    public Projectile Projectile;
    public WeaponData WeaponData;

    public bool IsFiring => _isFiring;
    private bool _isFiring;

    private void Awake()
    {
        Projectile.Exploded += OnExploded;
    }

    public void Fire(Vector2 aimDirection)
    {
        _isFiring = true;
        Projectile.gameObject.transform.position = (Vector2)transform.position + aimDirection.normalized * Constants.ProjectileOffset;
        Projectile.Fire(aimDirection * WeaponData.FireStrength.RandomValue);
    }

    private void OnExploded()
    {
        StartCoroutine(WaitUntilFiringFinished());
    }

    private IEnumerator WaitUntilFiringFinished()
    {
        while (Projectile.ExplodedCharacters.Any(c => c.IsAlive && c.IsMoving))
        {
            yield return null;
        }
        _isFiring = false;
    }
}
