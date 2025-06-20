using System.Collections.Generic;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models.Data.Player;
using Unity.Services.Leaderboards;
using UnityEngine;
using static UGSBootstrapper;


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
