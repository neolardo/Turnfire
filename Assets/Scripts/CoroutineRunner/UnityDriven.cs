using System.Collections;
using UnityEngine;

public abstract class UnityDriven
{
    protected MonoBehaviour _coroutineRunner;
    protected UnityDriven(MonoBehaviour coroutineRunner)
    {
        _coroutineRunner = coroutineRunner;
    }

    protected Coroutine StartCoroutine(IEnumerator routine)
    {
        return _coroutineRunner.StartCoroutine(routine);
    }
}
