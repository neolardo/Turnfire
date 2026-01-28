using Unity.Netcode;

public struct NetworkItemInstanceData : INetworkSerializable
{
    public int InstanceId;
    public int DefinitionId;
    public int Quantity;

    public NetworkItemInstanceData(ItemInstance itemInstance)
    {
        InstanceId = itemInstance.InstanceId;
        DefinitionId = itemInstance.DefinitionId;
        Quantity = itemInstance.Quantity;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer)
        where T : IReaderWriter
    {
        serializer.SerializeValue(ref InstanceId);
        serializer.SerializeValue(ref DefinitionId);
        serializer.SerializeValue(ref Quantity);
    }

    public ItemInstance ToItemInstance()
    {
        return new ItemInstance(
            this.InstanceId,
            this.DefinitionId,
            this.Quantity
        );
    }
}
