using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CharacterHealthbarRenderer : MonoBehaviour
{
    private float _initialScale;

    private void Awake()
    {
        _initialScale = transform.localScale.x;
    }

    public void SetCurrentHealth(float healthRatio)
    {
        transform.localScale = new Vector3(_initialScale * healthRatio, transform.localScale.y, transform.localScale.z);
    }    

}
