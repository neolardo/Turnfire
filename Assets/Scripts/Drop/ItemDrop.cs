using System;
using UnityEngine;

[Serializable] 
public class ItemDrop
{
    [Range(0, 1)] public float Probability;
    public ItemDefinition ItemDefinition;
}
