using UnityEngine;

public abstract class DatabaseItemScriptableObject : ScriptableObject
{
    public int Id { get; private set; }

    public void AssignId(int id) 
    { 
        Id = id;
    }
}
