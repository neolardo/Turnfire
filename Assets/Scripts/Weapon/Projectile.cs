using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    public ProjectileData ProjectileData;
    private Rigidbody2D _rb;

    public event Action Exploded;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag(Constants.GroundTag) || collision.CompareTag(Constants.CharacterTag) || collision.CompareTag(Constants.DeadZoneTag))
        {
            Explode();
        }
    }


    //TODO: interface for things that can be fired?
    public void Fire(Vector2 aimDirection)
    {
        gameObject.SetActive(true);
        _rb.linearVelocity = Vector2.zero;
        _rb.AddForce(aimDirection * ProjectileData.FireStrength, ForceMode2D.Impulse);
        //StartCoroutine(ShowBulletUntilImpact());
    }

    //TODO: delete
    private void DrawDebugCircle(Vector2 center, float radius, int segments, Color color)
    {
        float angleStep = 360f / segments;
        Vector3 prevPoint = center + new Vector2(Mathf.Cos(0), Mathf.Sin(0)) * radius;

        for (int i = 1; i <= segments; i++)
        {
            float rad = Mathf.Deg2Rad * (i * angleStep);
            Vector3 newPoint = center + new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * radius;

            Debug.DrawLine(prevPoint, newPoint, color);
            prevPoint = newPoint;
        }
    }

    private void Explode()
    {
        var mask = (LayerMask) (1 << Constants.CharacterLayer);
        Collider2D[] hits = Physics2D.OverlapCircleAll((Vector2)transform.position, ProjectileData.ExplosionRadius, mask);
        DrawDebugCircle(transform.position, ProjectileData.ExplosionRadius, 12, Color.green);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out Character character))
            {
                Debug.Log("Character hit!");
                var pushVector = (character.transform.position - transform.position) / ProjectileData.ExplosionRadius;
                character.Damage(ProjectileData.Damage);
                character.Push(pushVector * ProjectileData.ExplosionStrength);
            }
        }
        gameObject.SetActive(false);
        Debug.Log("Exploded!");
        Exploded?.Invoke();
    }
}
