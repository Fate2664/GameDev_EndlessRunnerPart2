using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class CloudBootstrap : MonoBehaviour
{
    // This script initializes Unity Services and signs in the user anonymously
    async void Awake()
    {
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();  //signed in anonymously
        }
    }
}
