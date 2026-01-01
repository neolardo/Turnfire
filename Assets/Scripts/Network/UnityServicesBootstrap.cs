using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;

public static class UnityServicesBootstrap
{
    private static bool _initialized;

    public static async Task InitializeAsync()
    {
        if (_initialized || UnityServices.State == ServicesInitializationState.Initialized)
            return;

        string profile = Guid.NewGuid().ToString("N").Substring(0, 20);

        await UnityServices.InitializeAsync(new InitializationOptions()
            .SetProfile(profile)
            .SetEnvironmentName("production"));

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log($"Signed in anonymously. PlayerID: {AuthenticationService.Instance.PlayerId}");
        }
        else
        {
            Debug.Log($"Already signed in with PlayerID: {AuthenticationService.Instance.PlayerId}");
        }
        _initialized = true;
    }
}
