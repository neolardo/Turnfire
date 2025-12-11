using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashSpriteAnimator : MonoBehaviour
{
    public bool IsFlashing { get; private set; }
    public void Flash(IEnumerable<SpriteRenderer> renderers, Color color, float inTime, float outTime)
    {
        StopAllCoroutines();
        foreach (var renderer in renderers)
        {
            StartCoroutine(FlashRoutine(renderer, color, inTime, outTime));
        }
    }

    private IEnumerator FlashRoutine(SpriteRenderer renderer, Color color, float inTime, float outTime)
    {
        IsFlashing = true;
        float t = 0f;
        var originalColor = renderer.color;

        while (t < 1f)
        {
            t += Time.deltaTime / inTime;
            renderer.color = Color.Lerp(originalColor, color, t);
            yield return null;
        }

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / outTime;
            renderer.color = Color.Lerp(color, originalColor, t);
            yield return null;
        }
        IsFlashing = false;
    }

}
