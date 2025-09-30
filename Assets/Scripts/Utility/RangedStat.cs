using System;
using UnityEngine;

[Serializable]
public abstract class RangedStat<T> where T: struct
{
    [SerializeField] [Tooltip("Inclusive minimum value.")] private T _min;
    [SerializeField] [Tooltip("Inclusive maximum value.")]  private T _max;
    public T Min => _min;
    public T Max => _max;

    public RangedStat(T min, T max)
    {
        _min = min;
        _max = max;
    }
}

[Serializable]
public class RangedStatInt : RangedStat<int>
{
    public RangedStatInt(int min, int max) : base(min, max) { }

    public int Avarage => (Min + Max) / 2;

    public int RandomValue => UnityEngine.Random.Range(Min, Max+1);
}

[Serializable]
public class RangedStatFloat : RangedStat<float>
{
    public RangedStatFloat(float min, float max) : base(min, max) { }

    public float Avarage => (Min + Max) / 2f;

    public float RandomValue => UnityEngine.Random.Range(Min, Max);
}

