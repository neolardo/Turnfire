using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Team : MonoBehaviour
{
    [SerializeField] private Color _teamColor;
    public bool IsTeamAlive => _characters.Any(c => c.IsAlive);
    public int NumAliveCharacters => _characters.Count(c => c.IsAlive);
    public float NormalizedTeamHealth => _characters.Sum(c => c.NormalizedHealth) / _characters.Count;
    public Color TeamColor => _teamColor;
    public string TeamName { get; private set; }

    private List<Character> _characters;
    public Character CurrentCharacter => _characters[_characterIndex];
    private int _characterIndex;

    public IGameplayInputSource InputSource { private set; get; }

    public event Action<float> TeamHealthChanged;
    public event Action TeamLost;


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
        InputSource = GameplayInputSourceFactory.Create(inputType, gameObject);
    }

    private void OnAnyTeamCharacterHealthChanged()
    {
        TeamHealthChanged?.Invoke(NormalizedTeamHealth);
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
