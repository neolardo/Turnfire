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
    private Team CurrentTeam => _teams[_teamIndex];


    public AlternativelyDoCharacterActionsForAllTeamsTurnState(MonoBehaviour manager, CharacterActionManager characterActionManager, List<Team> teams) : base(manager)
    {
        _characterActionManager = characterActionManager;
        _characterActionManager.CharacterActionsFinished += OnCharacterActionsFinished;
        _teams = teams;
        foreach (var team in teams)
        {
            _numCharactersActedThisRoundPerTeamDict.Add(team, 0);
            team.TeamHealthChanged += OnTeamHealthChanged;
        }
    }

    public override void OnDestroy()
    {
        foreach(var team in _teams)
        {
            team.TeamHealthChanged -= OnTeamHealthChanged;
        }
    }

    private void OnTeamHealthChanged(float health)
    {
        if(_teams == null || _teams.Count == 0)
            return;
        UpdateMaxCharactersToActPerRound();
    }

    private void UpdateMaxCharactersToActPerRound()
    {
        _maxCharactersToActPerRound = _teams.Where(t=> t.IsTeamAlive).Min(t => t.NumAliveCharacters);
    }

    public override void StartState()
    {
        base.StartState();
        UpdateMaxCharactersToActPerRound();
        ResetCharactersActedDictionary();
        SwitchToFirstTeam();
        StartActionsWithCurrentTeam();
    }

    private void OnCharacterActionsFinished()
    {
        _numCharactersActedThisRoundPerTeamDict[CurrentTeam]++;
        if (TrySwitchToNextTeam())
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

        do
        {
            _teamIndex = (_teamIndex + 1) % _teams.Count;
        } while ((!CurrentTeam.IsTeamAlive || _numCharactersActedThisRoundPerTeamDict[CurrentTeam] >= _maxCharactersToActPerRound) && _teamIndex != startTeamIndex);

        if (_teamIndex != startTeamIndex)
        {
            Debug.Log("Switched to team: " + CurrentTeam.TeamName);
            return true;
        }
        return false;
    }

}
