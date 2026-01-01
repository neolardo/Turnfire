using Unity.Services.Core;
using Unity.Services.Authentication;
using UnityEngine;
using System.Threading.Tasks;

public static class UnityServicesBootstrap
{
    private static bool _initialized;

    public static async Task InitializeAsync()
    {
        if (_initialized)
            return;

        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log($"Signed in anonymously. PlayerID: {AuthenticationService.Instance.PlayerId}");
        }

        _initialized = true;
    }
}
