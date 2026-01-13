using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PackageContainer))]
public class OfflineDropManager : MonoBehaviour, IDropManager
{
    [SerializeField] private OfflinePackage _offlinePackagePrefab;
    private List<IPackage> _currentPackages;
    private List<DropZone> _dropZones;

    public event System.Action AllPackagesLanded;

    private const float DelayAfterAllPackagesSpawned = 1f;

    private DropLogic _logic;
    private PackageContainer _container;

    private void Start()
    {
        _currentPackages = new List<IPackage>();
        _dropZones = FindFirstObjectByType<DropZoneContainer>().GetDropZones().ToList();
        if (_dropZones.Count == 0)
        {
            Debug.LogWarning("No drop zones to drop from.");
        }
        _logic = new DropLogic();
        _container = GetComponent<PackageContainer>();
        GameServices.Register(this);
    }

    public void TrySpawnPackages()
    {
        StartCoroutine(SpawnPackagesOneByOneAndWaitForAllOfThemToLand());
    }

    private IEnumerator SpawnPackagesOneByOneAndWaitForAllOfThemToLand()
    {
        int numDrops = _logic.CalculateRandomizedNumberOfDrops();

        for (int i = 0; i < numDrops; i++)
        {
            var package = _logic.CreatePackage(_offlinePackagePrefab, _dropZones, _container.transform);
            package.gameObject.SetActive(true);
            package.Destroyed += OnPackageDestroyed;
            _currentPackages.Add(package);
            yield return WaitForPackageToLand(package);
        }
        if (numDrops > 0)
        {
            yield return new WaitForSeconds(DelayAfterAllPackagesSpawned);
        }
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
        return _currentPackages;
    }
   
}
