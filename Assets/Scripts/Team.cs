using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Team : MonoBehaviour
{
    [HideInInspector] public bool IsTeamAlive => _characters.Any(c => c.IsAlive);
    [SerializeField] private List<Character> _characters;
    public Character CurrentCharacter => _characters[_characterIndex];
    private int _characterIndex;
    public string TeamName => _teamName;
    private string _teamName;

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
        }
        _characterIndex = 0;
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
