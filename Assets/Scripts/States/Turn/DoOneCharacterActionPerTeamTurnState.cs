using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DoOneCharacterActionPerTeamTurnState : TurnState
{
    public override TurnStateType State => TurnStateType.DoOneCharacterActionPerTeamTurnState;

    private List<Team> _teams;
    private int _teamIndex;
    private Dictionary<Team, bool> _teamsPlayedThisRoundDict = new Dictionary<Team, bool>();
    private Team CurrentTeam => _teams[_teamIndex];
    private CharacterActionManager _characterActionManager;

    public DoOneCharacterActionPerTeamTurnState(MonoBehaviour manager, CharacterActionManager characterActionManager, List<Team> teams) : base(manager)
    {
        _characterActionManager = characterActionManager;
        _characterActionManager.CharacterActionsFinished += OnCurrentTeamActionsFinished;
        _teams = teams;
        foreach (var team in teams)
        {
            _teamsPlayedThisRoundDict.Add(team, false);
        }
    }

    public override void StartState()
    {
        base.StartState();
        SwithToFirstAliveTeam();
        ResetTeamsPlayedDictionary();
        StartActionsWithCurrentTeam();
    }

    private void OnCurrentTeamActionsFinished()
    {
        _teamsPlayedThisRoundDict[CurrentTeam] = true;
        SwitchToNextAliveTeam();
        if (_teamsPlayedThisRoundDict[CurrentTeam])
        {
            EndState();
        }
        else
        {
            StartActionsWithCurrentTeam();
        }
    }

    private void StartActionsWithCurrentTeam()
    {
        CurrentTeam.SelectNextCharacter();
        _characterActionManager.StartActionsWithCharacter(CurrentTeam.CurrentCharacter);
    }

    private void ResetTeamsPlayedDictionary()
    {
        foreach (var key in _teamsPlayedThisRoundDict.Keys.ToList())
        {
            _teamsPlayedThisRoundDict[key] = false;
        }
    }

    private void SwithToFirstAliveTeam()
    {
        _teamIndex = -1;
        SwitchToNextAliveTeam();
    }

    private void SwitchToNextAliveTeam()
    {
        int aliveTeamCount = _teams.Count(t => t.IsTeamAlive);
        if(aliveTeamCount <= 1)
        {
            return;
        }

        do
        {
            _teamIndex = (_teamIndex + 1) % _teams.Count;
        } while (!_teams[_teamIndex].IsTeamAlive);
        Debug.Log("Switched to team: " + CurrentTeam.TeamName);
    }

}
