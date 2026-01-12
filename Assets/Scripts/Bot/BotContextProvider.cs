using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BotContextProvider : MonoBehaviour
{
    private DestructibleTerrainManager _destructibleTerrain;
    private JumpGraphManager _jumpGraphManager;
    private IEnumerable<Team> _teams;

    private void Awake()
    {
        _destructibleTerrain = FindFirstObjectByType<DestructibleTerrainManager>();
        _jumpGraphManager = FindFirstObjectByType<JumpGraphManager>();
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
        var currentCharacter = GameServices.TurnStateManager.GetCurrentCharacterInTeam(botTeam);
        var teamMates = botTeam.GetAllCharacters().Where(c => c != currentCharacter && c.IsAlive);
        var enemies = _teams.Where(t=> t != botTeam).Select(t => t.GetAllCharacters().Where(c => c.IsAlive)).Aggregate((t1, t2) => t1.Concat(t2));
        return new BotContext(action, currentCharacter, teamMates, enemies, GameServices.DropManager.GetAllAvailablePackages(), _destructibleTerrain, _jumpGraphManager.JumpGraph);
    }

}
