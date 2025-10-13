using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Team : MonoBehaviour
{
    [HideInInspector] public bool IsTeamAlive => _characters.Any(c => c.IsAlive);
    [SerializeField] private List<Character> _characters;
    [SerializeField] private Color _teamColor; //TODO

    public Color TeamColor => _teamColor;
    public Character CurrentCharacter => _characters[_characterIndex];
    private int _characterIndex;
    public string TeamName => _teamName;
    private string _teamName;

    /// <summary>
    /// Fires an event containing the normalized team health.
    /// </summary>
    public event Action<float> TeamHealthChanged;
    public event Action TeamLost;

    private void Awake()
    {
        if (_characters == null || _characters.Count == 0)
        {
            Debug.LogWarning($"There are no characters set for player: {gameObject.name}.");
        }
        _teamName = gameObject.name; //TODO
        foreach (Character character in _characters)
        {
            character.Died += OnAnyTeamCharacterDied;
            character.HealthChanged += (_) => OnAnyTeamCharacterHealthChanged();
        }
        _characterIndex = 0;
    }

    private void OnAnyTeamCharacterHealthChanged()
    {
        TeamHealthChanged?.Invoke(_characters.Sum(c => c.NormalizedHealth) / _characters.Count);
    }

    public void SelectNextCharacter()
    {
        do
        {
            _characterIndex = (_characterIndex + 1) % _characters.Count;
        } while (!_characters[_characterIndex].IsAlive);
    }

    private void OnAnyTeamCharacterDied()
    {
        if(!IsTeamAlive)
        {
            TeamLost?.Invoke();
        }
    }

}
