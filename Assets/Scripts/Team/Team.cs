using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(OnlineHumanTeamInputSource))]
[RequireComponent(typeof(OfflineHumanTeamInputSource))]
[RequireComponent(typeof(OnlineBotTeamInputSource))]
[RequireComponent(typeof(OfflineBotTeamInputSource))]
public class Team : MonoBehaviour, IConditionalEnumerable
{
    [SerializeField] private Color _teamColor;

    [SerializeField] private List<Character> _characters;
    public string TeamName { get; private set; }
    public int TeamId { get; private set; }
    public bool IsTeamAlive => _characters.Any(c => c.IsAlive);
    public ITeamInputSource InputSource { get; private set; }
    public Color TeamColor => _teamColor;
    public float NormalizedTeamHealth => _characters.Sum(c => c.NormalizedHealth) / _characters.Count;
    public int NumAliveCharacters => _characters.Count(c => c.IsAlive);
    public bool EnumeratorCondition => IsTeamAlive;
    public bool IsTeamInitialized => _characters == null ? false: _characters.All(c => c.IsInitialized);

    public event Action<float> TeamHealthChanged;
    public event Action TeamLost;

    public void Initialize(ITeamInputSource inputSource, int teamId, string teamName)
    {
        InputSource = inputSource;
        TeamId = teamId;
        TeamName = teamName;
        foreach(var character in _characters)  
        {
            character.Died += OnAnyTeamCharacterDied;
            character.HealthChanged += (_, _) => OnAnyTeamCharacterHealthChanged();
            CharacterComposer.Compose(character, this);
        }
        if (_characters.Count == 0)
        {
            Debug.LogWarning($"{TeamName} has 0 characters.");
        }
    }

    private void OnAnyTeamCharacterHealthChanged()
    {
        TeamHealthChanged?.Invoke(NormalizedTeamHealth);
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
