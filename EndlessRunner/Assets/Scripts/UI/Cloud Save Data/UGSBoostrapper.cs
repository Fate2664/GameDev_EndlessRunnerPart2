using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;

public class UGSBootstrapper : MonoBehaviour
{
    public bool autoLoadLeaderboardOnStart = true;
    public LeaderboardFetcher leaderboardFetcher;

    private async void Start()
    {
        await UGSInitializer.EnsureInitializedAsync();
        
        if (autoLoadLeaderboardOnStart && leaderboardFetcher != null)
        {
            leaderboardFetcher.LoadAndDisplayLeaderboard();
        }
    }
}

