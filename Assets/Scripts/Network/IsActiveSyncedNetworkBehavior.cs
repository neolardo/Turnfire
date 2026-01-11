using Unity.Netcode;

public abstract class IsActiveSyncedNetworkBehavior : NetworkBehaviour
{
    private NetworkVariable<bool> _isActive = new NetworkVariable<bool>();

    protected virtual void OnEnable()
    {
        if(IsServer)
        {
            _isActive.Value = true;
        }
    }

    protected virtual void OnDisable()
    {
        if (IsServer)
        {
            _isActive.Value = false;
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if(!IsServer)
        {
            gameObject.SetActive(_isActive.Value);
            _isActive.OnValueChanged += OnIsActiveChanged;
        }
    }

    private void OnIsActiveChanged(bool oldValue, bool newValue)
    {
        gameObject.SetActive(newValue);
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer)
        {
            _isActive.OnValueChanged -= OnIsActiveChanged;
        }
        base.OnNetworkDespawn();
    }


}
