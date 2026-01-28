using System.Linq;
using Unity.Netcode;
using UnityEngine;

public static class CharacterComposer
{
    public static void Compose(Character character, Team team)
    {
        if (character.IsInitialized)
            return;

        bool isOnline = GameplaySceneSettingsStorage.Current.IsOnlineGame;

        ICharacterState state;
        ICharacterPhysics physics;

        if (isOnline)
        {
            state = character.GetComponent<OnlineCharacterState>();
            physics = character.GetComponent<OnlineCharacterPhysics>();
            SpawnCharacterNetworkObject(team);
        }
        else
        {
            state = character.GetComponent<OfflineCharacterState>();
            physics = character.GetComponent<OfflineCharacterPhysics>();
        }
        (state as MonoBehaviour).enabled = true;
        (physics as MonoBehaviour).enabled = true;

        character.Initialize(team, state, physics);
    }

    private static void SpawnCharacterNetworkObject(Team team)
    {
        if (!NetworkManager.Singleton.IsServer)
            return;

        var netObj = team.GetComponent<NetworkObject>();
        var playerClientId = GameplaySceneSettingsStorage.Current.Players.First(p => p.TeamIndex == team.TeamId).ClientId;
        if (!netObj.IsSpawned)
        {
            netObj.Spawn();
        }
        if (netObj.OwnerClientId != playerClientId)
        {
            netObj.ChangeOwnership(playerClientId);
        }
    }
}
