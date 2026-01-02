using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkSceneLoader : NetworkBehaviour
{
    public static NetworkSceneLoader Instance { get; private set; }
    public GameplaySceneSettings CurrentGameplaySceneSettings
    {
        get 
        {
            return _networkSceneSettings.Value.IsValid ? _networkSceneSettings.Value.ToSceneSettings(_mapLocator) : null;
        }
        set
        {
            if(!IsServer)
            {
                return;
            }
            _networkSceneSettings.Value = NetworkGameplaySceneSettingsData.ToNetworkData(value);
        }
    }

    private NetworkVariable<NetworkGameplaySceneSettingsData> _networkSceneSettings = new (
                 new NetworkGameplaySceneSettingsData() { IsValid = false},
                 NetworkVariableReadPermission.Everyone,
                 NetworkVariableWritePermission.Server
            );

    private LoadingTextUI _loadingText;
    private MapLocator _mapLocator;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        if (IsServer)
        {
            DontDestroyOnLoad(gameObject);
        }
        _loadingText = FindFirstObjectByType<LoadingTextUI>(FindObjectsInactive.Include);
        NetworkManager.Singleton.SceneManager.OnLoad += OnSceneLoadStarted;
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnSceneLoadCompleted;
    }

    private void OnSceneLoadStarted(ulong clientId, string sceneName, LoadSceneMode loadSceneMode, AsyncOperation asyncOperation)
    {
        _loadingText.gameObject.SetActive(true);
        Debug.Log($"Started loading scene: {sceneName}");
    }

    private void OnSceneLoadCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        _loadingText = FindFirstObjectByType<LoadingTextUI>(FindObjectsInactive.Include);
        if(sceneName != Constants.MenuSceneName)
        {
            _mapLocator = FindFirstObjectByType<MapLocator>();
        }
        Debug.Log($"Finished loading scene: {sceneName}");
    }


    public void LoadMenuScene()
    {
        if(!IsServer)
        {
            return;
        }
        NetworkManager.Singleton.SceneManager.LoadScene(Constants.MenuSceneName, LoadSceneMode.Single);
    }

    public void LoadGameplayScene(GameplaySceneSettings settings)
    {
        if (!IsServer)
        {
            return;
        }
        CurrentGameplaySceneSettings = settings;
        NetworkManager.Singleton.SceneManager.LoadScene(settings.Map.SceneName, LoadSceneMode.Single);
    }

    public void ReloadScene()
    {
        if (!IsServer)
        {
            return;
        }
        var sceneName = SceneManager.GetActiveScene().name;
        NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
