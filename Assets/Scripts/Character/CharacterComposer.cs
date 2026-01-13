using System.Linq;
using Unity.Netcode;
using Unity.Netcode.Components;
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
            character.GetComponent<NetworkObject>().enabled = true;
            character.GetComponent<NetworkTransform>().enabled = true;
            character.GetComponent<NetworkRigidbody2D>().enabled = true;
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

        var netObj = character.GetComponent<NetworkObject>();
        if (netObj.IsSpawned)
            return;

        ulong ownerClientId =
            GameplaySceneSettingsStorage.Current.Players
                .First(p => p.TeamIndex == team.TeamId)
                .ClientId;

        netObj.SpawnAsPlayerObject(ownerClientId, true);
    }
}
