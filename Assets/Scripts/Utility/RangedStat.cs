using System;
using UnityEngine;

[Serializable]
public abstract class RangedStat
{
    [SerializeField][Range(0f, 1f)][Tooltip("Normalized value")] protected float _normalizedValue;
    [SerializeField] protected bool _isRandomized;
    [SerializeField][Range(0f, 1f)] protected float _randomness;
    public float NormalizedValue => _normalizedValue;
    public abstract float NormalizedDislayValue { get; }
    public abstract RangedStatGroupDefinition Group { get; }
}

[Serializable]
public abstract class RangedStat<T> : RangedStat where T: struct
{
    public abstract T AvarageValue { get; }
    public abstract T MinimumValue { get; }
    public abstract T MaximumValue { get; }
    public abstract T CalculateValue(); 
}

[Serializable]
public class RangedStatInt : RangedStat<int>
{
    [SerializeField] private RangedStatIntGroupDefinition _group;

    public override RangedStatGroupDefinition Group => _group;

    public override int AvarageValue => Mathf.RoundToInt(Mathf.Lerp(_group.Minimum, _group.Maximum, NormalizedValue));
    public override int MinimumValue
    {
        get
        {
            if(!_isRandomized)
            {
                return AvarageValue;
            }

            float deltaMax = NormalizedValue > 0.5f ? NormalizedValue : 1f - NormalizedValue;
            var lerpValue = Mathf.Max(NormalizedValue - deltaMax * _randomness, 0f);
            return Mathf.RoundToInt(Mathf.Lerp(_group.Minimum, _group.Maximum, lerpValue));
        }
    }
    public override int MaximumValue
    {
        get
        {
            if (!_isRandomized)
            {
                return AvarageValue;
            }

            float deltaMax = NormalizedValue > 0.5f ? NormalizedValue : 1f - NormalizedValue;
            var lerpValue = Mathf.Min(NormalizedValue + deltaMax * _randomness, 1f);
            return Mathf.RoundToInt(Mathf.Lerp(_group.Minimum, _group.Maximum, lerpValue));
        }
    }

    public override float NormalizedDislayValue => AvarageValue / (float)_group.Maximum;

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

    public override float AvarageValue => Mathf.Lerp(_group.Minimum, _group.Maximum, NormalizedValue);
    public override float MinimumValue
    {
        get
        {
            if (!_isRandomized)
            {
                return AvarageValue;
            }

            float deltaMax = NormalizedValue > 0.5f ? NormalizedValue : 1f - NormalizedValue;
            var lerpValue = Mathf.Max(NormalizedValue - deltaMax * _randomness, 0f);
            return Mathf.Lerp(_group.Minimum, _group.Maximum, lerpValue);
        }
    }
    public override float MaximumValue
    {
        get
        {
            if (!_isRandomized)
            {
                return AvarageValue;
            }

            float deltaMax = NormalizedValue > 0.5f ? NormalizedValue : 1f - NormalizedValue;
            var lerpValue = Mathf.Min(NormalizedValue + deltaMax * _randomness, 1f);
            return Mathf.Lerp(_group.Minimum, _group.Maximum, lerpValue);
        }
    }
    public override float NormalizedDislayValue => AvarageValue / _group.Maximum;

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

