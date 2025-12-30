using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BotContextProvider : MonoBehaviour
{
    private DropManager _dropManager;
    private DestructibleTerrainManager _destructibleTerrain;
    private JumpGraphManager _jumpGraphManager;
    private GameStateManager _gameStateManager;
    private IEnumerable<Team> _teams;

    private void Awake()
    {
        _dropManager = FindFirstObjectByType<DropManager>();
        _destructibleTerrain = FindFirstObjectByType<DestructibleTerrainManager>();
        _jumpGraphManager = FindFirstObjectByType<JumpGraphManager>();
        _gameStateManager = FindFirstObjectByType<GameStateManager>();
    }

    private void InitializeTeams()
    {
        _teams = FindObjectsByType(typeof(Team), FindObjectsInactive.Exclude, FindObjectsSortMode.None).Select(o => o as Team);
    }

    public BotContext CreateContext(Team botTeam, CharacterActionStateType action)
    {
        if (_teams == null)
        {
            InitializeTeams();
        }
        var teamMates = botTeam.GetAllCharacters().Where(c => c != botTeam.CurrentCharacter && c.IsAlive);
        var enemies = _teams.Where(t=> t != botTeam).Select(t => t.GetAllCharacters().Where(c => c.IsAlive)).Aggregate((t1, t2) => t1.Concat(t2));
        return new BotContext(action, botTeam.CurrentCharacter, teamMates, enemies, _dropManager.GetAllAvailablePackages(), _destructibleTerrain, _jumpGraphManager.JumpGraph, _gameStateManager);
    }

}
