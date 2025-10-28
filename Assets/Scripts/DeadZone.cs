using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        TryKillAndDestroyEverything(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        TryKillAndDestroyEverything(collision);
    }

    private void TryKillAndDestroyEverything(Collider2D collision)
    {
        if (collision.CompareTag(Constants.CharacterTag))
        {
            var c = collision.GetComponent<Character>();
            c.Kill();
        }
        else if (collision.CompareTag(Constants.PackageTag))
        {
            var p = collision.GetComponent<Package>();
            p.Destroy();
        }
        else if (collision.CompareTag(Constants.ProjectileTag))
        {
            var p = collision.GetComponent<Projectile>();
            p.ForceExplode();
        }
    }
}
