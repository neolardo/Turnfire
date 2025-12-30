using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(LineRendererFragment))]
public class PixelTrajectoryRenderer : MonoBehaviour
{
    [SerializeField] private int _maxSegments = 50;
    [SerializeField] private float _maxLength = 2.5f;
    [SerializeField] private float _stepTime = 0.05f;

    private LineRendererFragment _renderer;
    private float _trajectoryMultiplier;
    private Transform _origin;
    private bool _useGravity = true;
    private List<Vector2> _points;
    private Vector2 _originOffset;


    private void Awake()
    {
        _renderer = GetComponent<LineRendererFragment>();
        _points = new List<Vector2>();
    }

    public void SetTrajectoryMultipler(float multiplier)
    {
        _trajectoryMultiplier = multiplier;
    }
    public void SetOrigin(Transform origin, Vector2 offset = default)
    {
        _origin = origin;
        _originOffset = offset;
    }

    public void ToggleGravity(bool state)
    { 
        _useGravity = state;
    }

    public void HideTrajectory()
    {
        _renderer.Clear();
    }

    public void DrawTrajectory(Vector2 aimVector)
    {
        aimVector *= _trajectoryMultiplier;

        if (_useGravity)
        {
            DrawCurvedTrajectory(aimVector);
        }
        else
        {
            DrawStraightTrajectory(aimVector);
        }
    }

    private void DrawStraightTrajectory(Vector2 aimVector)
    {
        Vector2 start = (Vector2)_origin.position + _originOffset;
        Vector2 end = start + aimVector.normalized * _maxLength;

        _renderer.DrawLine(new[] { start, end });
    }

    private void DrawCurvedTrajectory(Vector2 aimVector)
    {
        Vector2 gravity = _useGravity ? Physics2D.gravity : Vector2.zero;

        Vector2 startPos = (Vector2)_origin.position + _originOffset;
        Vector2 lastPos = startPos;
        _points.Clear();
        _points.Add(startPos);

        float elapsed = 0f;
        float totalDistance = 0f;
        int count = 1;

        while (count < _maxSegments && totalDistance < _maxLength)
        {
            elapsed += _stepTime;
            Vector2 newPos = startPos + aimVector * elapsed + 0.5f * gravity * (elapsed * elapsed);

            float segmentLength = Vector2.Distance(lastPos, newPos);
            totalDistance += segmentLength;

            if (totalDistance > _maxLength)
            {
                float remaining = _maxLength - totalDistance;
                newPos = Vector2.Lerp(lastPos, newPos, remaining / segmentLength);
            }

            _points.Add(newPos);
            lastPos = newPos;

            count++;
        }

        _renderer.DrawLine(_points);
    }


}
