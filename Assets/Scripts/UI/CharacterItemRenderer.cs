using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CharacterItemRenderer : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
}
