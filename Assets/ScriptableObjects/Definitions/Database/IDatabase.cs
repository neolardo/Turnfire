public interface IDatabase<T>
{
    void Initialize();
    T GetById(int id);
}
