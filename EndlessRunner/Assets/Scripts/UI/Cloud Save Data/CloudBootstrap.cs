using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class CloudBootstrap : MonoBehaviour
{
    async void Awake()
    {
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();  //signed in anonymously
        }
    }
}
