using System.Collections.Generic;
using UnityEngine;

public interface IPool<T>
{
    T Get();
    T GetAndPlace(Vector2 position);
    void Release(T item);
    void ReleaseAll();
    IEnumerable<T> GetMultiple(int count);
}
