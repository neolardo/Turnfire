using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    [SerializeField] private GameplaySettingsDefinition _gameplaySettings;
    [SerializeField] private CountdownTimerUI _countdownTimer;
    [SerializeField] private TurnManager _turnManager;
    private bool _countdownEnded;


    private GameStateType _state;

    public GameStateType CurrentState
    {
        get
        {
            return _state;
        }

        private set
        {
            if (_state != value)
            {
                _state = value;
                StateChanged?.Invoke(_state);
            }
        }
    }

    public event Action <GameStateType> StateChanged;

    private void Awake()
    {
        var inputManager = FindFirstObjectByType<LocalGameplayInput>();
        var turnManager = FindFirstObjectByType<TurnManager>();
        inputManager.TogglePauseGameplayPerformed += OnTogglePauseResumeGameplay;
        inputManager.PrepareForGameStart();
        _countdownTimer.TimerEnded += OnCountdownEnded;
        turnManager.GameEnded += OnGameOver;
    }

    private void Start()
    {
        StartCoroutine(StartGameAfterCountdown());
    }

    private IEnumerator StartGameAfterCountdown()
    {
        var settings = SceneLoader.Instance.CurrentGameplaySceneSettings;
        var currentSceneName = settings.Map.SceneName;
        for (var mapIndex = 0; mapIndex<= 2; mapIndex++)
        {
            if (System.Environment.GetCommandLineArgs().Contains($"-map{mapIndex}") && currentSceneName != $"Map{mapIndex}")
            {
                settings.Map = FindFirstObjectByType<MapLocator>().GetMap(mapIndex);
                SceneLoader.Instance.LoadGameplayScene(settings);
                yield break;
            }
        }
       
       
        if (System.Environment.GetCommandLineArgs().Contains("-normalSim"))
        {
            _countdownTimer.StartTimer();
            yield return new WaitUntil(() => _countdownEnded);
            yield return new WaitForSeconds(_gameplaySettings.DelaySecondsAfterCountdown);
        }
        else
        {
            Time.timeScale = 15f;
            Application.targetFrameRate = 20;
            QualitySettings.SetQualityLevel(0);
            yield return new WaitForSeconds(.5f);
        }
        _countdownTimer.gameObject.SetActive(false);
        yield return new WaitUntil(()=> _turnManager.IsInitialized);
        StartGame();
    }
    private void StartGame()
    {
        CurrentState = GameStateType.Playing;
        _turnManager.StartGame(SceneLoader.Instance.CurrentGameplaySceneSettings);
    }

    public void OnCountdownEnded()
    {
        _countdownEnded = true;
    }

    private void OnGameOver(Team winnerTeam)
    {
        CurrentState = GameStateType.GameOver;
    }

    private void OnTogglePauseResumeGameplay()
    {
        if(CurrentState == GameStateType.Playing)
        {
            CurrentState = GameStateType.Paused;
        }
        else
        {
            CurrentState = GameStateType.Playing;
        }
    }

}
