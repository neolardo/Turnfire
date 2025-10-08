using UnityEngine;

public class CoroutineRunner : MonoBehaviour
{
    private static CoroutineRunner _instance;
    public static CoroutineRunner Instance
    {
        get
        {
            if (_instance == null)
            {
                var obj = new GameObject("GlobalCoroutineRunner");
                _instance = obj.AddComponent<CoroutineRunner>();
                Object.DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }
}
