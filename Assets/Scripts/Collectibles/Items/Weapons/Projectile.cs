using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    public ExplosionData ExplosionData;
    private Rigidbody2D _rb;

    public event Action Exploded;

    [HideInInspector] public List<Character> ExplodedCharacters;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        ExplodedCharacters = new List<Character>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag(Constants.GroundTag) || collision.CompareTag(Constants.CharacterTag) || collision.CompareTag(Constants.DeadZoneTag))
        {
            Explode();
        }
    }


    //TODO: interface for things that can be fired?
    public void Fire(Vector2 fireVector)
    {
        gameObject.SetActive(true);
        _rb.linearVelocity = Vector2.zero;
        _rb.AddForce(fireVector, ForceMode2D.Impulse);
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
        ExplodedCharacters.Clear();
        var mask = (LayerMask) (1 << Constants.CharacterLayer) | (1 << Constants.GroundLayer);
        var explosionRadius = ExplosionData.ExplosionRadius.RandomValue;
        var explosionStrength = ExplosionData.ExplosionStrength.RandomValue;
        var explosionDamage = ExplosionData.Damage.RandomValue;
        Collider2D[] hits = Physics2D.OverlapCircleAll((Vector2)transform.position, explosionRadius, mask);
        DrawDebugCircle(transform.position, explosionRadius, 12, Color.green);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out Character character))
            {
                var pushVector = (character.transform.position - transform.position) / explosionRadius;
                character.Damage(explosionDamage);
                character.Push(pushVector * explosionStrength);
                ExplodedCharacters.Add(character);
            }
            else if (hit.TryGetComponent(out DestructibleTerrain destTerrain))
            {
                destTerrain.ApplyExplosion(transform.position, explosionRadius);
            }
        }
        gameObject.SetActive(false);
        Exploded?.Invoke();
    }



}
