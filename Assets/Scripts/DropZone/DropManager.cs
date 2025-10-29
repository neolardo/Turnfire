using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropManager : MonoBehaviour
{
    [SerializeField] private GameplaySettingsDefinition _gameplaySettings;

    private List<Package> _currentPackages;
    private List<DropZone> _dropZones;
    private CameraController _cameraController;

    public event Action AllPackagesLanded;

    private const float DelayAfterAllPackagesSpawned = 1f;

    private void Awake()
    {
        _cameraController = FindFirstObjectByType<CameraController>();
        _currentPackages = new List<Package>();
        _dropZones = new List<DropZone>();
        for(int i = 0; i< transform.childCount; i++)
        {
            var dropZone = transform.GetChild(i).GetComponent<DropZone>();
            _dropZones.Add(dropZone);
        }
        if(_dropZones.Count == 0)
        {
            Debug.LogWarning("No drop zones to drop from.");
        }
        if (_gameplaySettings.PossibleDrops.Length == 0)
        {
            Debug.LogWarning("No packages available to drop.");
        }
    }

    public void SpawnPackages()
    {
        if (_gameplaySettings.PossibleDrops.Length == 0)
            return;

        StartCoroutine(SpawnPackagesOneByOneAndWaitForAllOfThemToLand());
    }

    private IEnumerator SpawnPackagesOneByOneAndWaitForAllOfThemToLand()
    {
        int numDrops = UnityEngine.Random.Range(_gameplaySettings.MinimumNumberOfDropsPerRound, _gameplaySettings.MaximumNumberOfDropsPerRound + 1);
        for (int i = 0; i < numDrops; i++)
        {
            int zoneIndex = UnityEngine.Random.Range(0, _dropZones.Count);
            var zone = _dropZones[zoneIndex];
            var package = zone.DropRandomPackage(_gameplaySettings.PossibleDrops);
            _currentPackages.Add(package);
            yield return WaitForPackageToLand(package);
        }
        if(numDrops > 0)
        {
            yield return new WaitForSeconds(DelayAfterAllPackagesSpawned);
        }
        AllPackagesLanded?.Invoke();
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
   
}
