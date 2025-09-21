using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class Healthbar : MonoBehaviour
{
    private int _currentHealth;
    private int _maxHealth;
    private Transform _target;
    private RectTransform _rect;
    private float _initialSize;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _initialSize = _rect.sizeDelta.x;
    }
    public void Follow(Transform target) //TODO
    {
        _target = target;
    }

    private void Update()
    {
        if (_target != null) //TODO
        {
            transform.position = (Vector2)Camera.main.WorldToScreenPoint(_target.position) + Vector2.up * Constants.VerticalHealthbarOffset;
        }
    }

    public void SetMaxHealth(int maxHealth)
    {
        _maxHealth = maxHealth;
        _currentHealth = maxHealth;
    }

    public void SetCurrentHeath(int currentHealth)
    {
        _currentHealth = currentHealth;
        Debug.Log("Current health: "+ _currentHealth);
        Debug.Log("Max health: "+ _maxHealth);
        _rect.sizeDelta = new Vector2(_initialSize * currentHealth / (float)_maxHealth, _rect.sizeDelta.y);
    }    

}
