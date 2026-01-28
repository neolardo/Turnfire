using System;
using UnityEngine;

public interface IPackage 
{
    bool IsMoving { get; }
    bool IsActiveInHierarchy { get; }
    Transform Transform { get; }

    event Action<IPackage> Destroyed;
    void SetItem(ItemInstance itemInstance);
    void Destroy();
}
