using System.Collections.Generic;
using UnityEngine;

public class DropZoneContainer : MonoBehaviour
{
    private List<DropZone> _dropZones;

    private void Awake()
    {
        _dropZones = new List<DropZone>();
        for (int i = 0; i < transform.childCount; i++)
        {
            var dropZone = transform.GetChild(i).GetComponent<DropZone>();
            _dropZones.Add(dropZone);
        }
    }
    public IList<DropZone> GetDropZones()
    {
        return _dropZones;
    }
}
