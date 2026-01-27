using UnityEngine;

public class MusicProvider : MonoBehaviour
{
    [SerializeField] private MusicDefinition _currentMusic;
    private void Start()
    {
        AudioManager.Instance.PlayMusic(_currentMusic.Clip);
    }
}
