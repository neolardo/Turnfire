using UnityEngine;

public class SimplePoolable : MonoBehaviour, IPoolable
{
    public virtual void OnCreatedInPool()
    {
        gameObject.SetActive(false);
    }

    public virtual void OnGotFromPool()
    {
        gameObject.SetActive(true);
    }

    public virtual void OnReleasedBackToPool()
    {
        gameObject.SetActive(false);
    }
}
