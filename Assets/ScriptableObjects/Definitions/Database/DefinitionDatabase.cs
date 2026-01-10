using System.Collections.Generic;
using UnityEngine;

public abstract class DefinitionDatabase<T> : ScriptableObject, IDatabase<T> where T : DatabaseItemScriptableObject
{
    [SerializeField] private List<T> _definitions;
    private Dictionary<int, T> _definitionsByIdDict;

    public void Initialize()
    {
        _definitionsByIdDict = new Dictionary<int, T>();
        int id = 0;
        foreach (var def in _definitions)
        {
            def.AssignId(id);
            _definitionsByIdDict.Add(def.Id, def);
            id++;
        }
    }

    public T GetById(int id)
    {
        return _definitionsByIdDict[id];
    }

}
