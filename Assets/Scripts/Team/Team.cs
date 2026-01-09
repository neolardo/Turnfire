using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Team : MonoBehaviour, IConditionalEnumerable
{
    [SerializeField] private Color _teamColor;

    private List<Character> _characters;
    private CyclicConditionalEnumerator<Character> _characterEnumerator;
    private bool _isSelected;
    public string TeamName { get; private set; }
    public int TeamId { get; private set; }
    public bool IsTeamAlive => _characters.Any(c => c.IsAlive);
    public ITeamInputSource InputSource { get; private set; }
    public Color TeamColor => _teamColor;
    public Character CurrentCharacter => _characterEnumerator.Current;
    public float NormalizedTeamHealth => _characters.Sum(c => c.NormalizedHealth) / _characters.Count;
    public int NumAliveCharacters => _characters.Count(c => c.IsAlive);

    public bool EnumeratorCondition => IsTeamAlive;

    public event Action<float> TeamHealthChanged;
    public event Action TeamLost;
    public event Action<bool> TeamSelectedChanged;


    public void Initialize(ITeamInputSource inputSource, int teamId, string teamName)
    {
        InputSource = inputSource;
        TeamId = teamId;
        TeamName = teamName;
        _characters = new List<Character>();
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            var character = child.GetComponent<Character>();
            character.Died += OnAnyTeamCharacterDied;
            character.HealthChanged += (_, _) => OnAnyTeamCharacterHealthChanged();
            CharacterComposer.Compose(character, this);
            _characters.Add(character);
        }
        if (_characters.Count == 0)
        {
            Debug.LogWarning($"{TeamName} has 0 characters.");
        }
        _characterEnumerator = new CyclicConditionalEnumerator<Character>(_characters);
        _characterEnumerator.Reset();
    }

    public void SelectTeam()
    {
        if( !_isSelected) 
        {
            _isSelected = true;
            TeamSelectedChanged?.Invoke(_isSelected);
        }
    }

    public void DeselectTeam()
    {
        if (_isSelected)
        {
            _isSelected = false;
            TeamSelectedChanged?.Invoke(_isSelected);
        }
    }

    private void OnAnyTeamCharacterHealthChanged()
    {
        TeamHealthChanged?.Invoke(NormalizedTeamHealth);
    }

    public void SelectNextCharacter()
    {
        _characterEnumerator.MoveNext(out var _);
    }

    private void OnAnyTeamCharacterDied()
    {
        if(!IsTeamAlive)
        {
            TeamLost?.Invoke();
        }
    }

    public IEnumerable<Character> GetAllCharacters() => _characters;

}
