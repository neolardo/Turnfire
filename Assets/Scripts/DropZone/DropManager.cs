using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropManager : MonoBehaviour
{
    private List<Package> _currentPackages;
    private List<DropZone> _dropZones;
    private CameraController _cameraController;

    public event System.Action AllPackagesLanded;

    private const float DelayAfterAllPackagesSpawned = 1f;

    private MapDefinition _currentMap;

    private void Awake()
    {
        _cameraController = FindFirstObjectByType<CameraController>();
        _currentPackages = new List<Package>();
        _dropZones = new List<DropZone>();
        for (int i = 0; i < transform.childCount; i++)
        {
            var dropZone = transform.GetChild(i).GetComponent<DropZone>();
            _dropZones.Add(dropZone);
        }
        if (_dropZones.Count == 0)
        {
            Debug.LogWarning("No drop zones to drop from.");
        }
       
    }
    private void Start()
    {
        _currentMap = SceneLoader.Instance.CurrentGameplaySceneSettings.Map;
        if (_currentMap.PossibleDrops.Length == 0)
        {
            Debug.LogWarning("No packages available to drop.");
        }
        foreach (var drop in _currentMap.PossibleDrops)
        {
            if (drop.ItemDefinition is ArmorDefinition armorDef)
            {
                armorDef.InitializeAnimations(); //TODO: move
            }
        }
    }

    public void TrySpawnPackages()
    {
        if (_currentMap.PossibleDrops.Length == 0)
            return;

        StartCoroutine(SpawnPackagesOneByOneAndWaitForAllOfThemToLand());
    }

    private IEnumerator SpawnPackagesOneByOneAndWaitForAllOfThemToLand()
    {
        int numDrops = CalculateNumberOfDrops();

        for (int i = 0; i < numDrops; i++)
        {
            int zoneIndex = Random.Range(0, _dropZones.Count);
            var zone = _dropZones[zoneIndex];
            var item = PickItem(_currentMap.PossibleDrops);
            var package = zone.DropItemInPackage(item);
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

    private int CalculateNumberOfDrops()
    {
        int numDrops = 0;
        bool drop = Random.value < _currentMap.FirstDropChance;
        while (drop)
        {
            numDrops++;
            drop = Random.value < _currentMap.MultipleDropChance && numDrops + 1 <= _currentMap.MaxDropsPerRound;
        }
        return numDrops;
    }

    public static ItemDefinition PickItem(IList<ItemDrop> drops)
    {
        if (drops == null || drops.Count == 0)
            return null;

        float totalWeight = 0f;

        for (int i = 0; i < drops.Count; i++)
        {
            totalWeight += Mathf.Max(0f, drops[i].Probability);
        }

        if (totalWeight <= 0f)
            return null;

        float roll = Random.value * totalWeight;
        float cumulative = 0f;

        for (int i = 0; i < drops.Count; i++)
        {
            cumulative += drops[i].Probability;
            if (roll <= cumulative)
            {
                return drops[i].ItemDefinition;
            }
        }

        return drops[drops.Count - 1].ItemDefinition;
    }

    private void OnPackageDestroyed(Package package)
    {
        _currentPackages.Remove(package);
    }

    private IEnumerator WaitForPackageToLand(Package package)
    {
        _cameraController.SetPackageTarget(package);
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate(); // wait for gravity to be applied
        while (package != null && package.gameObject.activeInHierarchy && package.IsMoving)
        {
            yield return null;
        }
    }

    public IEnumerable<Package> GetAllAvailablePackages()
    {
        return _currentPackages;
    }
   
}
