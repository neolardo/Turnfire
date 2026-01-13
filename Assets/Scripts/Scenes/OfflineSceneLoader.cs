using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OfflineSceneLoader : MonoBehaviour, ISceneLoader 
{
    public static OfflineSceneLoader Instance { get; private set; }
    public GameplaySceneSettings CurrentGameplaySceneSettings => GameplaySceneSettingsStorage.Current;

    private void Awake()
    {
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
        GameServices.Register(this);
    }

    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        yield return null;
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        while (!op.isDone)
        {
            yield return null;
        }
        Debug.Log("Scene reload finished");
    }

    public void LoadMenuScene()
    {
        GameplaySceneSettingsStorage.Current = null;
        StartCoroutine(LoadSceneCoroutine(Constants.MenuSceneName));
    }

    public void LoadGameplayScene(GameplaySceneSettings settings)
    {
        GameplaySceneSettingsStorage.Current = settings;
        StartCoroutine(LoadSceneCoroutine(CurrentGameplaySceneSettings.Map.SceneName));
    }

    public void ReloadScene()
    {
        var scene = SceneManager.GetActiveScene();
        StartCoroutine(LoadSceneCoroutine(scene.name));
    }
}
