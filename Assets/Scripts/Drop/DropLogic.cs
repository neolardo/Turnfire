using System.Collections.Generic;
using UnityEngine;

public class DropLogic
{
    private MapDefinition _currentMap;
    public DropLogic()
    {
        _currentMap = GameplaySceneSettingsStorage.Current.Map;
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

    public int CalculateRandomizedNumberOfDrops()
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

    public T CreatePackage<T>(T packagePrefab, IList<DropZone> dropZones) where T: Object, IPackage
    {
        var itemDefinition = PickRandomItem();
        int zoneIndex = Random.Range(0, dropZones.Count);
        var spawnPos = dropZones[zoneIndex].GetRandomPoint();
        var package = GameObject.Instantiate(packagePrefab, spawnPos, Quaternion.identity);
        package.SetItem(ItemInstance.CreateAsDrop(itemDefinition));
        return package;
    }

    private ItemDefinition PickRandomItem()
    {
        var drops = _currentMap.PossibleDrops;
        if (drops == null || drops.Length == 0)
            return null;

        float totalWeight = 0f;

        for (int i = 0; i < drops.Length; i++)
        {
            totalWeight += Mathf.Max(0f, drops[i].Probability);
        }

        if (totalWeight <= 0f)
            return null;

        float roll = Random.value * totalWeight;
        float cumulative = 0f;

        for (int i = 0; i < drops.Length; i++)
        {
            cumulative += drops[i].Probability;
            if (roll <= cumulative)
            {
                return drops[i].ItemDefinition;
            }
        }

        return drops[drops.Length - 1].ItemDefinition;
    }

}
