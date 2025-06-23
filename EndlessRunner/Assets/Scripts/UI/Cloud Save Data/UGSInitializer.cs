using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;
using UnityEngine;

public static class UGSInitializer
{
    // This static class initializes Unity Gaming Services (UGS) and signs in the user anonymously.
    private static bool _isInitializing = false;
    private static bool _initialized = false;

    public static async Task EnsureInitializedAsync()
    {
        if (_initialized || _isInitializing)
            return;

        _isInitializing = true;
        // Ensure that Unity Services is initialized and the user is signed in anonymously.
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            await UnityServices.InitializeAsync();
        }
        // Check if the user is already signed in
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        _initialized = true;
        _isInitializing = false;
    }
}
