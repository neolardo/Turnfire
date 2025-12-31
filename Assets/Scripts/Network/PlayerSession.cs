using Unity.Netcode;

public class PlayerSession : NetworkBehaviour
{
    public NetworkVariable<int> TeamId;
    public NetworkVariable<bool> IsReady;

  
}
