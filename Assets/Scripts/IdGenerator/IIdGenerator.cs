public interface IIdGenerator
{
    public const int InvalidId = -1;
    public const int InitialId = -1;
    int GenerateId();
}
