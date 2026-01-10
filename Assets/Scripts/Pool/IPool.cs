using System.Collections.Generic;

public interface IPool<T>
{
    T Get();
    void Release(T item);
    void ReleaseAll();
    IEnumerable<T> GetMultiple(int count);
}
