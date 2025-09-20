using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Team : MonoBehaviour
{
    [HideInInspector] public bool IsTeamAlive => _characters.Any(c => c.IsAlive);
    [SerializeField] private List<Character> _characters;
    public Character CurrentCharacter => _currentCharacter;
    private Character _currentCharacter;
    private int _characterIndex;

    public event Action TurnFinished;

    private void Awake()
    {
        if (_characters == null || _characters.Count == 0)
        {
            Debug.LogWarning($"There are no characters set for player: {gameObject.name}.");
        }
        foreach( var character in _characters )
        {
            character.TurnFinished += OnTurnFinished;
        }
    }

    public void StartTurn()
    {
        Debug.Log($"{gameObject.name}'s turn!");
        SelectNextCharacter();
    }

    private void OnTurnFinished()
    {
        _characterIndex = (_characterIndex+1) % _characters.Count;
        TurnFinished?.Invoke();
    }

    private void SelectNextCharacter()
    {
        while (!_characters[_characterIndex].IsAlive)
        {
            _characterIndex++;
        }
        _currentCharacter = _characters[_characterIndex];
        _currentCharacter.Select();
    }

}
