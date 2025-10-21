using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pool<T> : MonoBehaviour where T : Component
{
    [SerializeField] protected T _prefab;
    [SerializeField][Range(1, 100)] private int _initialSize = 10;
    [SerializeField] private Transform _container;

    private List<T> _available;
    private List<T> _inUse;

    protected virtual void Awake()
    {
        _available = new List<T>();
        _inUse = new List<T>();
        if (_prefab == null)
        {
            Debug.LogError($"Pool<{typeof(T).Name}>: Prefab is not assigned!", this);
            enabled = false;
            return;
        }

        if (_container == null)
        {
            _container = transform;
        }

        for (int i = 0; i < _initialSize; i++)
        {
            _available.Add(CreateInstance());
        }
    }

    protected virtual T CreateInstance()
    {
        T instance = Instantiate(_prefab, _container);
        instance.gameObject.SetActive(false);
        return instance;
    }

    public virtual T Get()
    {
        T item;
        if (_available.Count > 0)
        {
            item = _available[0];
            _available.RemoveAt(0);
        }
        else
        {
            item = CreateInstance();
        }

        _inUse.Add(item);
        item.gameObject.SetActive(true);
        return item;
    }

    public void Release(T item)
    {
        if (!_inUse.Contains(item))
        {
            Debug.LogWarning($"Trying to release an object not managed by this pool: {item.name}");
            return;
        }

        item.gameObject.SetActive(false);
        item.transform.SetParent(_container, false);

        _inUse.Remove(item);
        _available.Add(item);
    }

    public void ReleaseAll()
    {
        foreach (var item in _inUse.ToList())
        {
            Release(item);
        }
    }

    public IEnumerable<T> GetMultiple(int count)
    {
        var tempList = new List<T>();
        for (int i = 0; i < count; i++)
        {
            tempList.Add(Get());
        }
        return tempList;
    }
}
