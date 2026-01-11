using System;

public interface IPackage 
{
    bool IsMoving { get; }
    bool IsActiveInHierarchy { get; }

    event Action<IPackage> Destroyed;
    void SetItem(ItemInstance itemInstance);
    void Destroy();
}
