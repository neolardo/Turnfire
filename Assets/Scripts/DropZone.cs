using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class DropZone : MonoBehaviour
{
    [Header("Drop Settings")]
    [SerializeField] private List<Collectible> _possibleCollectibles;
    [SerializeField] private Package _packagePrefab;
    [SerializeField] private int _minDrops = 1;
    [SerializeField] private int _maxDrops = 3;

    private BoxCollider2D _zone;
    private List<Package> _currentPackages;
    private CameraController _cameraController;

    public event Action AllPackagesLanded;

    private void Awake()
    {
        _currentPackages = new List<Package>();
        _zone = GetComponent<BoxCollider2D>();
        _cameraController = FindFirstObjectByType<CameraController>();
        if (_possibleCollectibles == null || _possibleCollectibles.Count == 0)
        {
            Debug.LogWarning("No packages available to drop.");
        }
    }

    public void SpawnPackages()
    {
        if (_possibleCollectibles == null || _possibleCollectibles.Count == 0)
            return;

        StartCoroutine(SpawnPackagesOneByOneAndWaitForAllOfThemToLand());
   }

    private IEnumerator SpawnPackagesOneByOneAndWaitForAllOfThemToLand()
    {
        int numDrops = UnityEngine.Random.Range(_minDrops, _maxDrops + 1);
        for (int i = 0; i < numDrops; i++)
        {
            Vector2 spawnPos = GetRandomPointInZone();
            var collectible = _possibleCollectibles[UnityEngine.Random.Range(0, _possibleCollectibles.Count)];
            var package = Instantiate(_packagePrefab, spawnPos, Quaternion.identity);
            package.SetCollectible(collectible);
            package.gameObject.SetActive(true);
            _currentPackages.Add(package);
            yield return WaitForPackageToLand(package);
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

    private Vector2 GetRandomPointInZone()
    {
        Vector2 center = (Vector2)transform.position + _zone.offset;
        Vector2 size = _zone.size;

        float randomX = UnityEngine.Random.Range(center.x - size.x / 2f, center.x + size.x / 2f);
        float randomY = UnityEngine.Random.Range(center.y - size.y / 2f, center.y + size.y / 2f);

        return new Vector2(randomX, randomY);
    }
}
