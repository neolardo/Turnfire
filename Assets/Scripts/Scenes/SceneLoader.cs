using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }
    public GameplaySceneSettings CurrentGameplaySceneSettings { get; private set; }

    private LoadingTextUI _loadingText;

    private void Awake()
    {
        CurrentGameplaySceneSettings = new GameplaySceneSettings() { NumTeams = 2, SceneName = "Map0", UseTimer = true }; //TODO: delete
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        _loadingText = FindFirstObjectByType<LoadingTextUI>(FindObjectsInactive.Include);
    }


    private IEnumerator LoadSceneCoroutine(GameplaySceneSettings settings)
    {
        CurrentGameplaySceneSettings = settings;
        _loadingText.gameObject.SetActive(true);
        yield return null;
        AsyncOperation op = SceneManager.LoadSceneAsync(CurrentGameplaySceneSettings.SceneName);
        while (!op.isDone)
        {
            yield return null;
        }
    }

    public void LoadScene(GameplaySceneSettings settings)
    {
        StartCoroutine(LoadSceneCoroutine(settings));
    }
}
