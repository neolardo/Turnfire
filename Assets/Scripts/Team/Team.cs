using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Team : MonoBehaviour
{
    [HideInInspector] public bool IsTeamAlive => _characters.Any(c => c.IsAlive);
    [SerializeField] private Color _teamColor;

    private List<Character> _characters;
    public Color TeamColor => _teamColor;
    public Character CurrentCharacter => _characters[_characterIndex];
    private int _characterIndex;
    public string TeamName { get; private set; }

    public event Action<float> TeamHealthChanged;
    public event Action TeamLost;

    public int NumAliveCharacters => _characters.Count(c => c.IsAlive);

    public IGameplayInputSource InputSource => _inputSource;
    
    private IGameplayInputSource _inputSource;

    private void Awake()
    {
        TeamName = gameObject.name;
        _characters = new List<Character>();
        for (int i=0; i< transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            _characters.Add(child.GetComponent<Character>());
        }
        if(_characters.Count == 0)
        {
            Debug.LogWarning($"{TeamName} has 0 characters.");
        }

        foreach (Character character in _characters)
        {
            character.Died += OnAnyTeamCharacterDied;
            character.HealthChanged += (_, _) => OnAnyTeamCharacterHealthChanged();
            character.SetTeam(this);
        }
        _characterIndex = 0;
    }

    public void InitializeInputSource(InputSourceType inputType)
    {
        _inputSource = GameplayInputSourceFactory.Create(inputType, gameObject);
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

    public IEnumerable<Character> GetAllCharacters() => _characters;

}
