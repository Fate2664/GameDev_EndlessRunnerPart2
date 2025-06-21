using TMPro;
using UnityEngine;
using System.Collections.Generic;
using Unity.Services.CloudSave;
using Unity.Services.Leaderboards;
using UnityEngine.Events;

public class Score : MonoBehaviour
{
    //This script manages the scoring of the player

    public List<TextMeshProUGUI> ValueText;
    public bool DoublePointsActive;

    [Header("Events")]
    public UnityEvent OnScoreIncrement;

    private float score;


    public static Score Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;  //Singleton instance
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);  //Destroy duplicate instances
        }
    }

    private void OnEnable()
    {
        if (OnScoreIncrement == null)
        {
            OnScoreIncrement = new UnityEvent();  //Initialize the event if it's null
        }
        OnScoreIncrement.AddListener(IncrementScore);  //Add the IncrementScore method to the event
    }

    private void OnDisable()
    {
        if (OnScoreIncrement != null)
        {
            OnScoreIncrement.RemoveListener(IncrementScore);  //Remove the IncrementScore method from the event
        }
    }

    public void IncrementScore()
    {
        if (!DoublePointsActive)
        {
            score++;    //increment the score value
        }

        if (DoublePointsActive)
        {
            score += 2;    //increment the score value 
        }

        for (int i = 0; i < ValueText.Count; i++)
        {
            ValueText[i].text = score.ToString("0");       //Change the text to show the new score
        }

    }

    public async void SaveScore()
    {
        var data = new Dictionary<string, object> { { "player_score", score } };
        await CloudSaveService.Instance.Data.Player.SaveAsync(data);  //Save the score to the cloud

        try
        {
            await LeaderboardsService.Instance.AddPlayerScoreAsync("EndlessRunner_ScoreLeaderboard", score);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error submitting score to leaderboard: " + e.Message);
        }
    }

    public async void LoadScore()
    {
        var cloudData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { "player_score" });  //Load the score from the cloud
        if (cloudData.TryGetValue("player_score", out var cloudScore))
        {
            score = float.Parse(cloudScore.ToString());
            foreach (var text in ValueText)
            {
                text.text = score.ToString("0");  //Update the UI with the loaded score
            }
        }

    }

    public void ResetScore()
    {
        score = 0;  //Reset the score to zero
        for (int i = 0; i < ValueText.Count; i++)
        {
            ValueText[i].text = score.ToString("0");  //Update the UI to reflect the reset score
        }
    }


}
