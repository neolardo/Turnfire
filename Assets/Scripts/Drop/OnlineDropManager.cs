using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(PackageContainer))]
public class OnlineDropManager : NetworkBehaviour, IDropManager
{
    [SerializeField] private OnlinePackage _onlinePackagePrefab;
    private List<IPackage> _currentPackages;
    private List<DropZone> _dropZones;

    public event System.Action AllPackagesLanded;

    private const float DelayAfterAllPackagesSpawned = 1f;

    private DropLogic _logic;
    private PackageContainer _container;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _currentPackages = new List<IPackage>();
        _dropZones = FindFirstObjectByType<DropZoneContainer>().GetDropZones().ToList();
        if (_dropZones.Count == 0)
        {
            Debug.LogWarning("No drop zones to drop from.");
        }
        _logic = new DropLogic();
        _container = GetComponent<PackageContainer>();
    }

    public void TrySpawnPackages()
    {
        if(!IsServer)
        {
            return;
        }
        StartCoroutine(SpawnPackagesOneByOneAndWaitForAllOfThemToLand());
    }

    private IEnumerator SpawnPackagesOneByOneAndWaitForAllOfThemToLand()
    {
        int numDrops = _logic.CalculateRandomizedNumberOfDrops();

        for (int i = 0; i < numDrops; i++)
        {
            var package = _logic.CreatePackage(_onlinePackagePrefab, _dropZones, _container.transform);
            package.gameObject.SetActive(true);
            package.Destroyed += OnPackageDestroyed;
            var networkObj = package.GetComponent<NetworkObject>();
            networkObj.Spawn();
            _currentPackages.Add(package);
            yield return new WaitUntil(() => networkObj.IsSpawned);
            yield return WaitForPackageToLand(package);
        }
        if (numDrops > 0)
        {
            yield return new WaitForSeconds(DelayAfterAllPackagesSpawned);
        }
        BroadcastAllPackagesLandedClientRpc();
    }


    [Rpc(SendTo.Everyone, InvokePermission = RpcInvokePermission.Server)]
    private void BroadcastAllPackagesLandedClientRpc()
    {
        AllPackagesLanded?.Invoke();
    }

    private void OnPackageDestroyed(IPackage package)
    {
        _currentPackages.Remove(package);
    }

    private IEnumerator WaitForPackageToLand(IPackage package)
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate(); // wait for gravity to be applied
        while (package != null && package.IsActiveInHierarchy && package.IsMoving)
        {
            yield return null;
        }
    }

    public IEnumerable<IPackage> GetAllAvailablePackages()
    {
        if(!IsServer)
        {
            return null;
        }    
        return _currentPackages;
    }

}
