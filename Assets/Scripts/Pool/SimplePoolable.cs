using UnityEngine;

public class SimplePoolable : MonoBehaviour, IPoolable
{
    public virtual void OnCreatedInPool()
    {
        if (gameObject != null && gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }

    public virtual void OnGotFromPool()
    {
        if (gameObject != null && !gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
    }

    public virtual void OnReleasedBackToPool()
    {
        if(gameObject != null && gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }
}
