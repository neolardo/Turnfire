using UnityEngine;

public abstract class Collectible : MonoBehaviour
{
    public abstract bool TryCollect(Character c);
}
