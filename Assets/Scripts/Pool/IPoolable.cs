public interface IPoolable
{
    void OnCreatedInPool();
    void OnGotFromPool();
    void OnReleasedBackToPool();
}
