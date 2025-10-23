using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CharacterHealthbarRenderer : MonoBehaviour
{
    private float _unitScale;

    private void Awake()
    {
        _unitScale = transform.localScale.x;
    }

    public void SetCurrentHealth(float normalizedHealth, int health)
    {
        transform.localScale = new Vector3(_unitScale * health / (float)Constants.CharacterHealthbarValuePerUnit, transform.localScale.y, transform.localScale.z);
    }    

}
