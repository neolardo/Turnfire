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
        //TODO: delete before release
        //var players = new List<Player> { new Player(0, "Player1", PlayerType.Human), { new Player(1, "Player2", PlayerType.Human) } };
        //var map = FindFirstObjectByType<MapLocator>().Map0;
        //CurrentGameplaySceneSettings = new GameplaySceneSettings() { Players = players, Map = map, UseTimer = false, IsOnlineGame = true};
        
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
        _loadingText = FindFirstObjectByType<LoadingTextUI>(FindObjectsInactive.Include);
    }

    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        _loadingText.gameObject.SetActive(true);
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
        StartCoroutine(LoadSceneCoroutine(Constants.MenuSceneName));
    }

    public void LoadGameplayScene(GameplaySceneSettings settings)
    {
        CurrentGameplaySceneSettings = settings;
        StartCoroutine(LoadSceneCoroutine(CurrentGameplaySceneSettings.Map.SceneName));
    }

    public void ReloadScene()
    {
        var scene = SceneManager.GetActiveScene();
        StartCoroutine(LoadSceneCoroutine(scene.name));
    }
}
