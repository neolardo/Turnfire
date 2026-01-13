using UnityEngine;

public class OfflineIdGenerator : MonoBehaviour, IIdGenerator
{
    private int _lastId;

    private void Awake()
    {
        _lastId = IIdGenerator.InitialId;
    }
    private void Start()
    {
        GameServices.Register(this);
    }
    public int GenerateId()
    {
        _lastId++;
        return _lastId;
    }
}
