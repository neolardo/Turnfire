using System;
using UnityEngine;

[Serializable]
public abstract class RangedStat
{
    [SerializeField][Range(0f, 1f)][Tooltip("Normalized value")] protected float _normalizedValue;
    [SerializeField] protected bool _isRandomized;
    [SerializeField][Range(0f, 1f)] protected float _randomness;
    public float NormalizedValue => _normalizedValue;
    public abstract RangedStatGroupDefinition Group { get; }
}

[Serializable]
public abstract class RangedStat<T> : RangedStat where T: struct
{
    public abstract T CalculateValue(); 
}

[Serializable]
public class RangedStatInt : RangedStat<int>
{
    [SerializeField] private RangedStatIntGroupDefinition _group;

    public override RangedStatGroupDefinition Group => _group;

    public override int CalculateValue()
    {
        if(_isRandomized)
        {
            float deltaMax = NormalizedValue > 0.5f ? NormalizedValue : 1f - NormalizedValue;
            float normalizedMin = Mathf.Max(NormalizedValue - deltaMax * _randomness, 0f);
            float normalizedMax = Mathf.Min(NormalizedValue + deltaMax * _randomness, 1f);
            float normalizedRandom = UnityEngine.Random.Range(normalizedMin, normalizedMax);
            return Mathf.RoundToInt(Mathf.Lerp(_group.Minimum, _group.Maximum, normalizedRandom));
        }
        else
        {
            return Mathf.RoundToInt(Mathf.Lerp(_group.Minimum, _group.Maximum, NormalizedValue));  
        }
    }
}

[Serializable]
public class RangedStatFloat : RangedStat<float>
{
    [SerializeField] private RangedStatFloatGroupDefinition _group;
    public override RangedStatGroupDefinition Group => _group;

    public override float CalculateValue()
    {
        if (_isRandomized)
        {
            float deltaMax = NormalizedValue > 0.5f ? NormalizedValue : 1f - NormalizedValue;
            float normalizedMin = Mathf.Max(NormalizedValue - deltaMax * _randomness, 0f);
            float normalizedMax = Mathf.Min(NormalizedValue + deltaMax * _randomness, 1f);
            float normalizedRandom = UnityEngine.Random.Range(normalizedMin, normalizedMax);
            return Mathf.Lerp(_group.Minimum, _group.Maximum, normalizedRandom);
        }
        else
        {
            return Mathf.Lerp(_group.Minimum, _group.Maximum, NormalizedValue);
        }
    }
}

