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
        CurrentGameplaySceneSettings = new GameplaySceneSettings() { NumTeams = 2, NumBots= 1,  SceneName = "Map0", UseTimer = false }; //TODO: remove
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        _loadingText = FindFirstObjectByType<LoadingTextUI>(FindObjectsInactive.Include);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        BotEvaluationStatistics.Clear();
        _loadingText = FindFirstObjectByType<LoadingTextUI>(FindObjectsInactive.Include);
    }

    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        _loadingText.gameObject.SetActive(true);
        yield return null;
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone)
        {
            yield return null;
        }
    }

    public void LoadMenuScene()
    {
        StartCoroutine(LoadSceneCoroutine(Constants.MenuSceneName));
    }

    public void LoadGameplayScene(GameplaySceneSettings settings)
    {
        CurrentGameplaySceneSettings = settings;
        StartCoroutine(LoadSceneCoroutine(CurrentGameplaySceneSettings.SceneName));
    }

    public void ReloadScene()
    {
        var scene = SceneManager.GetActiveScene();
        StartCoroutine(LoadSceneCoroutine(scene.name));
    }
}
