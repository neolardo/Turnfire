using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class DropZone : MonoBehaviour
{
    [Header("Drop Settings")]
    [SerializeField] private List<Droppable> _possibleDrops;
    [SerializeField] private int _minDrops = 1;
    [SerializeField] private int _maxDrops = 3;

    private BoxCollider2D _zone;

    public event Action AllDropsLanded;
    private List<Droppable> _currentDrops;

    private void Awake()
    {
        _currentDrops = new List<Droppable>();
        _zone = GetComponent<BoxCollider2D>();
        if (_possibleDrops == null || _possibleDrops.Count == 0)
        {
            Debug.LogWarning("No drops are possible.");
        }
    }

    public void SpawnDrops()
    {
        if (_possibleDrops == null || _possibleDrops.Count == 0)
            return;

        int count = UnityEngine.Random.Range(_minDrops, _maxDrops + 1);

        for (int i = 0; i < count; i++)
        {
            Vector2 spawnPos = GetRandomPointInZone();
            var prefab = _possibleDrops[UnityEngine.Random.Range(0, _possibleDrops.Count)];
            var drop = Instantiate(prefab, spawnPos, Quaternion.identity);
            drop.gameObject.SetActive(true);
            _currentDrops.Add(drop);
        }

        StartCoroutine(WaitUntilAllDropsLanded());
    }

    private IEnumerator WaitUntilAllDropsLanded()
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate(); //waiting for gravity to be applied

        while (_currentDrops.Any(d => d.isActiveAndEnabled && d.IsMoving))
        {
            yield return null;
        }
        AllDropsLanded?.Invoke();
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
