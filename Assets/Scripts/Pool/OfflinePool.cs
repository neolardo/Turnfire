using UnityEngine;

public class OfflinePool<T> : PoolBase<T> where T : Component, IPoolable
{
}
