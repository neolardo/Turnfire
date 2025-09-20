using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Team : MonoBehaviour
{

    private Character _currentCharacter;
    public Character CurrentCharacter
    {
        get
        {
            return _currentCharacter;
        }
        private set
        {
            _currentCharacter = value;
            CharacterChanged?.Invoke(value);
        }

    }
    public event Action<Character> CharacterChanged;

    [HideInInspector] public bool IsTeamAlive => _characters.Any(c => c.IsAlive);
    [SerializeField] private List<Character> _characters;
    private int _characterIndex;

    private void Awake()
    {
        if (_characters == null || _characters.Count == 0)
        {
            Debug.LogWarning($"There are no characters set for player: {gameObject.name}.");
        }
    }

    private void Start()
    {
        CurrentCharacter = _characters[_characterIndex];
    }

    public void OnImpulseReleased(Vector2 impulse)
    {
        CurrentCharacter?.Jump(impulse);
    }

    private void StartTurn()
    {
        if(IsTeamAlive)
        {
            do
            {
                _characterIndex++;
            }
            while (!_characters[_characterIndex].IsAlive);
            CurrentCharacter = _characters[_characterIndex];
        }
    }

    private void EndTurn()
    {
    }

}
