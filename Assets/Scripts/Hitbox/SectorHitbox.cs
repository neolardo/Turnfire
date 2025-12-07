using System;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class SectorHitbox : MonoBehaviour
{
    private float _angle;
    private float _distance;
    private PolygonCollider2D _collider;
    private Collider2D[] _overlapColliders;

    private const float AngleResolution = 20f;

    public event Action<HitboxContactContext> Contacted;


    private void Awake()
    {
        _overlapColliders = new Collider2D[Constants.OverlapHitColliderNumMax];
    }

    public void Initialize(float angleDegrees, float distance)
    {
        if(_collider == null)
        {
            _collider = GetComponent<PolygonCollider2D>();
        }
        _angle = angleDegrees;
        _distance = distance;
        transform.rotation = Quaternion.identity;
        InitializeSectorPoints();
    }

    private void OnEnable()
    {
        CheckOverlap();
    }

    private void CheckOverlap()
    {
        var filter = new ContactFilter2D();
        filter.SetLayerMask(LayerMaskHelper.GetLayerMask(Constants.CharacterLayer));
        int count = Physics2D.OverlapCollider(_collider, filter, _overlapColliders);
        for (int i = 0; i < count; i++)
        {
            var collider = _overlapColliders[i];
            Contacted?.Invoke(new HitboxContactContext(collider.ClosestPoint(transform.position), collider));
        }
    }

    private void InitializeSectorPoints()
    {
        float halfAngle = _angle * 0.5f;
        float startAngle = -halfAngle;
        float endAngle = halfAngle;

        int resolution = Mathf.Max(1, Mathf.CeilToInt(_angle / AngleResolution));

        int pointCount = resolution + 2;
        Vector2[] points = new Vector2[pointCount];

        // origin is first point
        points[0] = Vector2.zero;

        for (int i = 0; i <= resolution; i++)
        {
            float t = (float)i / resolution;
            float currentAngle = Mathf.Lerp(startAngle, endAngle, t);
            float rad = currentAngle * Mathf.Deg2Rad;

            points[i + 1] = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * _distance;
        }

        _collider.points = points;
    }

    public void Rotate(Vector2 aimVector)
    {
        float angle = aimVector.ToAngleDegrees() * Mathf.Deg2Rad;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

}
