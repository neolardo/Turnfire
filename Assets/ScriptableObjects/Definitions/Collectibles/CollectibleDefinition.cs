using UnityEngine;

public abstract class CollectibleDefinition : ScriptableObject
{
    public abstract CollectibleType CollectibleType { get; }
}
