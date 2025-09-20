using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameRoundManager : MonoBehaviour
{
    [HideInInspector] public Team CurrentTeam;
    [SerializeField] private List<Team> _teams;
    [SerializeField] private TrajectoryRenderer _trajectoryRenderer;
    private int _teamIndex;

    private bool IsGameOver => _teams.Count(t => t.IsTeamAlive) == 1;

    void Awake()
    {
        if (_teams == null || _teams.Count == 0)
        {
            Debug.LogWarning("There are no teams.");
        }
        foreach (var team in _teams)
        {
            team.TurnFinished += EndTurn;
        }

        var inputManager = FindFirstObjectByType<InputManager>();
        inputManager.ImpulseReleased += OnImpulseReleased;
    }

    private void Start()
    {
        NewTurn();
    }

    private void NewTurn()
    {
        CurrentTeam = _teams[_teamIndex];
        CurrentTeam.StartTurn();
    }

    private void EndTurn()
    {
        if(IsGameOver)
        {
            EndGame();
        }
        do
        {
            _teamIndex = (_teamIndex + 1) % _teams.Count;
        } while (!_teams[_teamIndex].IsTeamAlive);
        NewTurn();
    }

    private void EndGame()
    {
        var winnerTeam = _teams.First(t => t.IsTeamAlive);
        //TODO
        Debug.Log("Game Over! Winner team: " + winnerTeam.gameObject.name);
    }

    private void OnImpulseReleased(Vector2 impulse)
    {
        CurrentTeam.CurrentCharacter.OnImpulseReleased(impulse);
    }


}
