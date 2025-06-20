using Nova;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LeaderboardItemVisuals : ItemVisuals
{
    public TextBlock UserNameLabel;
    public TextBlock ScoreLabel;
    public TextBlock PositionLabel;
    public UIBlock2D Background;
}

[System.Serializable]
public class LeaderboardEntryData
{
    public string UserName;
    public double Score;
    public int Position;

    public LeaderboardEntryData(string playerName, double score, int position)
    {
        UserName = playerName;
        Score = score;
        Position = position;
    }
}

[System.Serializable]
public class LeaderboardVisuals : ItemVisuals
{
    public UIBlock OptionsRoot;
    public ListView OptionsList;

    public Color PrimaryRowColor;
    public Color SecondaryRowColor;

    private bool initialized = false;

    public void ShowLeaderboard(List<LeaderboardEntryData> entries)
    {
        OptionsRoot.gameObject.SetActive(true);
        EnsureEventHandlers();
        OptionsList.SetDataSource(entries);
    }

    private void EnsureEventHandlers()
    {
        if (initialized)
            return;

        initialized = true;

        OptionsList.AddDataBinder<LeaderboardEntryData, LeaderboardItemVisuals>(BindItem);
    }

    private void BindItem(Data.OnBind<LeaderboardEntryData> evt, LeaderboardItemVisuals visuals, int index)
    {
        var data = evt.UserData;

        visuals.PositionLabel.Text = (data.Position + 1).ToString();
        visuals.UserNameLabel.Text = string.IsNullOrEmpty(data.UserName) ? "Anonymous" : data.UserName;
        visuals.ScoreLabel.Text = data.Score.ToString();

        visuals.Background.Color = index % 2 == 0 ? PrimaryRowColor : SecondaryRowColor;
    }
}

