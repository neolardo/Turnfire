using System.Linq;
using Unity.Netcode;

public static class CharacterComposer
{
    public static void Compose(Character character, Team team)
    {
        bool isOnline = GameplaySceneSettingsStorage.Current.IsOnlineGame;

        ICharacterState state;
        ICharacterPhysics physics;

        if (isOnline)
        {
            state = character.gameObject.AddComponent<OnlineCharacterState>();
            physics = character.gameObject.AddComponent<OnlineCharacterPhysics>();
        }
        else
        {
            state = character.gameObject.AddComponent<OfflineCharacterState>();
            physics = character.gameObject.AddComponent<OfflineCharacterPhysics>();
        }

        character.Initialize(team, state, physics);

        if (isOnline && NetworkManager.Singleton.IsServer)
        {
            SpawnCharacter(character, team);
        }
    }

    private static void SpawnCharacter(Character character, Team team)
    {
        var netObj = character.GetComponent<NetworkObject>();

        if (netObj.IsSpawned)
            return;

        ulong ownerClientId = GameplaySceneSettingsStorage.Current.Players
            .First(p => p.TeamIndex == team.TeamId)
            .ClientId;

        netObj.SpawnAsPlayerObject(ownerClientId);
    }
}
