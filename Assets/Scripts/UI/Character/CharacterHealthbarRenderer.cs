using System.Collections;
using UnityEngine;

public class CharacterHealthbarRenderer : MonoBehaviour
{
    [SerializeField] private CharacterAnimatorDefinition _animatorDefinition;
    [SerializeField] private Transform _permanentBar;
    [SerializeField] private Transform _temporaryBar;
    private float _unitScale;
    private int _lastHealth;
    private Vector3 _initialOffset;

    public void Initilaize(int initialHealth)
    {
        _unitScale = _permanentBar.localScale.x;
        _initialOffset = -HealthToTemporaryBarOffset(initialHealth / 2);
        _permanentBar.localPosition = _initialOffset;
        _temporaryBar.localPosition = _initialOffset;
        _permanentBar.localScale = HealthToScale(initialHealth);
        _temporaryBar.localScale = HealthToScale(0);
        _lastHealth = initialHealth;
    }

    private Vector3 HealthToScale(int health)
    {
        return new Vector3(HealthToSize(health), _permanentBar.localScale.y, _permanentBar.localScale.z);
    }

    private Vector3 HealthToTemporaryBarOffset(float health)
    {
        return new Vector3(HealthToSize(health), _permanentBar.localPosition.y, _permanentBar.localPosition.z);
    }

    private float HealthToSize(float health)
    {
       return _unitScale * health / (float)Constants.CharacterHealthbarValuePerUnit;
    }

    private IEnumerator AnimateHealthChange(int lastHealth, int newHealth)
    {
        int smallerHealth = Mathf.Min(lastHealth, newHealth);
        int largerHealth = Mathf.Max(lastHealth, newHealth);
        Vector3 temporaryStartScale = Vector3.zero;
        Vector3 temporaryTargetScale = HealthToScale(largerHealth - smallerHealth);
        Vector3 temporaryStartOffset = HealthToTemporaryBarOffset(lastHealth);
        Vector3 temporaryTargetOffset = HealthToTemporaryBarOffset(smallerHealth);
        _temporaryBar.localPosition = _initialOffset + temporaryStartOffset;
        float deltaTime = 0;
        while (deltaTime < _animatorDefinition.HealtbarSlideInSeconds)
        {
            float t = deltaTime / _animatorDefinition.HealtbarSlideInSeconds;
            _temporaryBar.localScale = Vector3.Lerp(temporaryStartScale, temporaryTargetScale, t);
            _temporaryBar.localPosition = _initialOffset + Vector3.Lerp(temporaryStartOffset, temporaryTargetOffset, t);
            deltaTime += Time.deltaTime;
            yield return null;
        }
        deltaTime = 0;
        temporaryStartOffset = temporaryTargetOffset;
        temporaryTargetOffset = HealthToTemporaryBarOffset(newHealth);
        (temporaryStartScale, temporaryTargetScale) = (temporaryTargetScale, temporaryStartScale);
        _permanentBar.localScale = HealthToScale(newHealth);
        while (deltaTime < _animatorDefinition.HealtbarSlideOutSeconds)
        {
            float t = deltaTime / _animatorDefinition.HealtbarSlideOutSeconds;
            _temporaryBar.localScale = Vector3.Lerp(temporaryStartScale, temporaryTargetScale, t);
            _temporaryBar.localPosition = _initialOffset + Vector3.Lerp(temporaryStartOffset, temporaryTargetOffset, t);
            deltaTime += Time.deltaTime;
            yield return null;
        }

    }

    public void SetCurrentHealth(float normalizedHealth, int health)
    {
        StopAllCoroutines();
        StartCoroutine(AnimateHealthChange(_lastHealth, health));
        _lastHealth = health;
    }    

}
