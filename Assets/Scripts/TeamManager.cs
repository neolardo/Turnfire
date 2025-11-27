using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    [SerializeField] private List<Team> _possibleTeams;
    private List<Team> _teams;

    private void Start()
    {
        if (_possibleTeams == null || _possibleTeams.Count == 0)
        {
            Debug.LogWarning("There are no teams.");
        }

        InitializeTeams();
        var turnManager = FindFirstObjectByType<TurnManager>();
        turnManager.Initialize(_teams);
    }

    private void InitializeTeams()
    {
        _teams = _possibleTeams.Take(SceneLoader.Instance.CurrentGameplaySceneSettings.NumTeams).ToList();
        for (int i = _teams.Count; i < _possibleTeams.Count; i++)
        {
            _possibleTeams[i].gameObject.SetActive(false);
        }

        var botManagerFactory = FindFirstObjectByType<BotManagerFactory>();
        bool first = true;//TODO: implement input initialization later
        foreach (var team in _teams)
        {
            if (first)
            {
                team.InitializeInputSource(InputSourceType.Local);
                first = false;
            }
            else
            {
                team.InitializeInputSource(InputSourceType.Bot);
                botManagerFactory.CreateBotForTeam(team, BotDifficulty.Easy); //TODO: difficulty based on scene loader settings
            }
        }
    }

}
