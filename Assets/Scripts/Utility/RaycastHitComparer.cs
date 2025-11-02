using System.Collections.Generic;

using UnityEngine;
public class RaycastHit2DComparer : IComparer<RaycastHit2D>
{
    public static readonly RaycastHit2DComparer Instance = new RaycastHit2DComparer();
    public int Compare(RaycastHit2D a, RaycastHit2D b)
    {
        return a.distance.CompareTo(b.distance);
    }
}