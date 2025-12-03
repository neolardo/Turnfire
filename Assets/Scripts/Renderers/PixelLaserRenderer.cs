using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LineRendererCompute))]
public class PixelLaserRenderer : MonoBehaviour
{
    [SerializeField] private float _updateRate = 0f;   // 0 = update every frame
    [SerializeField] private float _laserLength = 2f;
    [SerializeField] private float _animationDuration = 2f;
    private LineRendererCompute _renderer;
    private Coroutine _runningAnimation;

    private void Awake()
    {
        _renderer = GetComponent<LineRendererCompute>();
    }

    public void DrawLaser(Vector2[] worldPoints)
    {
        if (_runningAnimation != null)
        {
            StopCoroutine(_runningAnimation);
        }

        _runningAnimation = StartCoroutine(AnimateLaserLine(_laserLength, _animationDuration, worldPoints));
    }

    private IEnumerator AnimateLaserLine(float length, float duration, Vector2[] points)
    {
        // Precompute the cumulative lengths along the polyline
        float totalPathLength;
        float[] cumulative = ComputeCumulativeLengths(points, out totalPathLength);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float headDistance = Mathf.Lerp(0f, totalPathLength, t);
            float tailDistance = Mathf.Max(0f, headDistance - length);

            // Extract the moving segment as world-space pixel coordinates
            Vector2[] segment = ExtractSegment(points, cumulative, tailDistance, headDistance);

            // Send the new line points to the renderer
            _renderer.DrawLine(segment);

            // Frame pacing
            if (_updateRate <= 0f)
                yield return null;
            else
                yield return new WaitForSeconds(_updateRate);

            elapsed += Time.deltaTime;
        }

        // Ensure it ends fully drawn
        _renderer.DrawLine(points);
    }


    private float[] ComputeCumulativeLengths(Vector2[] points, out float total)
    {
        float[] cLengths = new float[points.Length];
        cLengths[0] = 0f;

        for (int i = 1; i < points.Length; i++)
        {
            cLengths[i] = cLengths[i - 1] + Vector2.Distance(points[i], points[i - 1]);
        }

        total = cLengths[cLengths.Length - 1];
        return cLengths;
    }


    private Vector2[] ExtractSegment( Vector2[] points, float[] cumulative, float startDistance,float endDistance)
    {
        // Worst case: segment spans entire polyline
        // Allocate small buffer only once per frame
        Vector2[] buffer = new Vector2[points.Length + 2];
        int count = 0;

        for (int i = 1; i < points.Length; i++)
        {
            float segStart = cumulative[i - 1];
            float segEnd = cumulative[i];

            // Segment outside requested area
            if (segEnd < startDistance || segStart > endDistance)
                continue;

            // Clamp interpolation ranges
            float localStart = Mathf.Clamp(startDistance, segStart, segEnd);
            float localEnd = Mathf.Clamp(endDistance, segStart, segEnd);

            float t1 = Mathf.InverseLerp(segStart, segEnd, localStart);
            float t2 = Mathf.InverseLerp(segStart, segEnd, localEnd);

            Vector2 p1 = Vector2.Lerp(points[i - 1], points[i], t1);
            Vector2 p2 = Vector2.Lerp(points[i - 1], points[i], t2);

            // Add points in order
            if (count == 0 || buffer[count - 1] != p1)
                buffer[count++] = p1;

            buffer[count++] = p2;
        }

        // Trim buffer
        Vector2[] result = new Vector2[count];
        for (int i = 0; i < count; i++)
            result[i] = buffer[i];

        return result;
    }
}