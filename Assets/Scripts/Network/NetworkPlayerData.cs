using Unity.Collections;
using Unity.Netcode;

public struct NetworkPlayerData : INetworkSerializable
{
    public ulong ClientId;
    public int TeamId;
    public FixedString32Bytes Name; // max length enforced
    public PlayerType Type;


    public void NetworkSerialize<T>(BufferSerializer<T> serializer)
        where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref TeamId);
        serializer.SerializeValue(ref Name);
        serializer.SerializeValue(ref Type);
    }

    public Player ToPlayer()
    {
        return new Player(
            this.ClientId,
            this.TeamId,
            this.Name.ToString(),
            this.Type
        );
    }
}