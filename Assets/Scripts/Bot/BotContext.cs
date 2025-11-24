using System.Collections.Generic;

public readonly struct BotContext
{
    public readonly CharacterActionStateType ActionState;
    public readonly Character Self;
    public readonly IEnumerable<Character> TeamMates;
    public readonly IEnumerable<Character> Enemies;
    public readonly IEnumerable<Package> Packages;
    public readonly DestructibleTerrainManager DestructibleTerrain;
    public readonly JumpGraph JumpGraph;

    public BotContext(CharacterActionStateType actionState, Character self, IEnumerable<Character> teamMates, IEnumerable<Character> enemies, IEnumerable<Package> packages, DestructibleTerrainManager destructibleTerrain, JumpGraph jumpGraph)
    {
        ActionState = actionState;
        Self = self;
        TeamMates = teamMates;
        Enemies = enemies;
        Packages = packages;
        DestructibleTerrain = destructibleTerrain;
        JumpGraph = jumpGraph;
    }

}
