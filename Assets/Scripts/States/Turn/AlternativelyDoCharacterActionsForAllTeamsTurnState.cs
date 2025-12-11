using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AlternativelyDoCharacterActionsForAllTeamsTurnState : TurnState
{
    public override TurnStateType State => TurnStateType.AlternativelyDoCharacterActionsForAllTeams;

    private CharacterActionManager _characterActionManager;
    private List<Team> _teams;
    private Dictionary<Team, int> _numCharactersActedThisRoundPerTeamDict = new Dictionary<Team, int>(); 
    private int _maxCharactersToActPerRound;
    private int _teamIndex;
    private bool _forceEndState;
    private Team CurrentTeam => _teams[_teamIndex];


    public AlternativelyDoCharacterActionsForAllTeamsTurnState(MonoBehaviour manager, CharacterActionManager characterActionManager, List<Team> teams) : base(manager)
    {
        _characterActionManager = characterActionManager;
        _characterActionManager.CharacterActionsFinished += OnCharacterActionsFinished;
        _teams = teams;
        foreach (var team in teams)
        {
            _numCharactersActedThisRoundPerTeamDict.Add(team, 0);
            team.TeamHealthChanged += (_) => UpdateMaxCharactersToActPerRound();
        }
    }

    private void UpdateMaxCharactersToActPerRound()
    {
        _maxCharactersToActPerRound = _teams.Where(t=> t.IsTeamAlive).Min(t => t.NumAliveCharacters);
    }

    public override void StartState()
    {
        _forceEndState = false;
        base.StartState();
        UpdateMaxCharactersToActPerRound();
        ResetCharactersActedDictionary();
        SwitchToFirstTeam();
        StartActionsWithCurrentTeam();
    }

    public override void ForceEndState()
    {
        _forceEndState = true;
        _characterActionManager.ForceEndActions();
    }

    private void OnCharacterActionsFinished()
    {
        _numCharactersActedThisRoundPerTeamDict[CurrentTeam]++;
        if (!_forceEndState && TrySwitchToNextTeam())
        {
            StartActionsWithCurrentTeam();
        }
        else
        {
            EndState();
        }
    }

    private void StartActionsWithCurrentTeam()
    {
        CurrentTeam.SelectNextCharacter();
        _characterActionManager.StartActionsWithCharacter(CurrentTeam.CurrentCharacter);
    }

    private void ResetCharactersActedDictionary()
    {
        foreach (var key in _numCharactersActedThisRoundPerTeamDict.Keys.ToList())
        {
            _numCharactersActedThisRoundPerTeamDict[key] = 0;
        }
    }

    private void SwitchToFirstTeam()
    {
        _teamIndex = -1;
        TrySwitchToNextTeam();
    }

    private bool TrySwitchToNextTeam()
    {
        var startTeamIndex = _teamIndex;
        if(_teamIndex >= 0)
        {
            CurrentTeam.DeselectTeam();
        }

        do
        {
            _teamIndex = (_teamIndex + 1) % _teams.Count;
        } while ((!CurrentTeam.IsTeamAlive || _numCharactersActedThisRoundPerTeamDict[CurrentTeam] >= _maxCharactersToActPerRound) && _teamIndex != startTeamIndex);

        if (_teamIndex != startTeamIndex)
        {
            CurrentTeam.SelectTeam();
            Debug.Log("Switched to team: " + CurrentTeam.TeamName);
            return true;
        }
        return false;
    }

}
