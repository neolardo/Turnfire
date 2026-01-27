using TMPro;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PoolableSpriteRenderer : SimplePoolable
{
    private SpriteRenderer _spriteRenderer;
    public SpriteRenderer SpriteRenderer
    {
        get 
        {
            if(_spriteRenderer == null)
            {
                _spriteRenderer = GetComponent<SpriteRenderer>();
            }
            return _spriteRenderer;
        }
    }
}
