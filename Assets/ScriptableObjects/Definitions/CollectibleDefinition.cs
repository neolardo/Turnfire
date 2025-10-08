using UnityEngine;

public abstract class CollectibleDefinition : ScriptableObject
{
    public abstract CollectibleType Type { get; }
}
