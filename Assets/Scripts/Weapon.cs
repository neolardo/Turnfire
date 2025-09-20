using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Projectile Projectile;
    public int NumProjectiles;

    public void Fire(Vector2 aimDirection)
    {
        Projectile.gameObject.transform.position = transform.position;
        Projectile.Fire(aimDirection);
    }
}
