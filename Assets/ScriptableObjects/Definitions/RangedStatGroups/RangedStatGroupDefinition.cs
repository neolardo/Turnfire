using UnityEngine;

public abstract class RangedStatGroupDefinition : ScriptableObject
{
    public string Name;
}

public abstract class RangedStatGroupDefinition<T> : RangedStatGroupDefinition
{
    [Tooltip("Inclusive minimum value")] public T Minimum;
    [Tooltip("Inclusive maximum value")] public T Maximum;
}