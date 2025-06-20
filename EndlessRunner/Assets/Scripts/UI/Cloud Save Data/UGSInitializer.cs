using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;
using UnityEngine;

public static class UGSInitializer
{
    private static bool _isInitializing = false;
    private static bool _initialized = false;

    public static async Task EnsureInitializedAsync()
    {
        if (_initialized || _isInitializing)
            return;

        _isInitializing = true;

        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            await UnityServices.InitializeAsync();
        }

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        _initialized = true;
        _isInitializing = false;
    }
}
