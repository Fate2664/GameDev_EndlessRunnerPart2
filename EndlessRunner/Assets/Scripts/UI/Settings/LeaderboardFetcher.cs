using System.Collections.Generic;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models.Data.Player;
using Unity.Services.Leaderboards;
using UnityEngine;
using static UGSBootstrapper;

// This script fetches the leaderboard data from Unity's Leaderboards service and displays it using the LeaderboardVisuals component.

public class LeaderboardFetcher : MonoBehaviour
{
    [SerializeField] private LeaderboardVisuals leaderboardUI;


    public void Start()
    {
        LoadAndDisplayLeaderboard();
    }
    public async void LoadAndDisplayLeaderboard()
    {
        try
        {

            var scoreResults = await LeaderboardsService.Instance.GetScoresAsync(
                "EndlessRunner_ScoreLeaderboard",
                new GetScoresOptions { Limit = 10 }
            );

            var entries = new List<LeaderboardEntryData>();
            int index = 0;
            //for each entry in the leaderboard results, create a LeaderboardEntryData object and add it to the entries list.
            foreach (var entry in scoreResults.Results)
            {
                string playerName = string.IsNullOrEmpty(entry.PlayerName) ? "Anonymous" : entry.PlayerName;
                entries.Add(new LeaderboardEntryData(playerName, entry.Score, index));  
                index++;
            }

            leaderboardUI.ShowLeaderboard(entries);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load leaderboard: {e.Message}");
        }
    }

}
