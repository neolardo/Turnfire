using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }
    public SceneLoadSettings SceneSettings { get; private set; }

    private LoadingTextUI _loadingText;

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
        _loadingText = FindFirstObjectByType<LoadingTextUI>(FindObjectsInactive.Include);
    }


    private IEnumerator LoadSceneCoroutine(SceneLoadSettings settings)
    {
        SceneSettings = settings;
        _loadingText.gameObject.SetActive(true);
        yield return null;
        AsyncOperation op = SceneManager.LoadSceneAsync(SceneSettings.SceneName);
        while (!op.isDone)
        {
            yield return null;
        }
    }

    public void LoadScene(SceneLoadSettings settings)
    {
        StartCoroutine(LoadSceneCoroutine(settings));
    }
}
