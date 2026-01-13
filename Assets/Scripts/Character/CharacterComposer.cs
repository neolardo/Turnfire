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
            SpawnCharacterNetworkObject(character, team);
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

    private static void SpawnCharacterNetworkObject(Character character, Team team)
    {
        if (!NetworkManager.Singleton.IsServer)
            return;

        ulong ownerClientId =
        GameplaySceneSettingsStorage.Current.Players
            .First(p => p.TeamIndex == team.TeamId)
            .ClientId;

        var netObj = character.GetComponent<NetworkObject>();
        if (netObj.IsSpawned)
        {
            netObj.ChangeOwnership(ownerClientId);
        }
        else
        {
            netObj.SpawnAsPlayerObject(ownerClientId, true);
        }
    }
}
