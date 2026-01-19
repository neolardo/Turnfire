using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PoolableSpriteRenderer : SimplePoolable
{
    public SpriteRenderer SpriteRenderer { get; private set; }

    private void Awake()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }
}
