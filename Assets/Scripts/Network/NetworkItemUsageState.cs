using Unity.Netcode;

public struct NetworkItemUsageState : INetworkSerializable
{
    public int ItemInstanceId;
    public NetworkItemUsageContextData UsageContext;
    public bool IsInUse;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ItemInstanceId);
        serializer.SerializeValue(ref UsageContext);
        serializer.SerializeValue(ref IsInUse);
    }

    public NetworkItemUsageState(ItemInstance item, ItemUsageContext context, bool inUse)
    {
        ItemInstanceId = item == null ? Constants.InvalidId : item.InstanceId;
        UsageContext = new NetworkItemUsageContextData(context);
        IsInUse = inUse;
    }

    public static NetworkItemUsageState CreateUsageStartedState(ItemInstance item, ItemUsageContext context)
    {
        return new NetworkItemUsageState(item, context, true);
    }
    public static NetworkItemUsageState CreateUsageFinishedState()
    {
        return new NetworkItemUsageState(null, default, false);
    }

}
