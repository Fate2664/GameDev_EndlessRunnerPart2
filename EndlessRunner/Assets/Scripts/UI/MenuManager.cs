    using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    public void StartGame()
    {
        if (Score.Instance != null)
        {
            Score.Instance.SaveScore();  //Save the score before starting a new game
            Destroy(Score.Instance.gameObject);  //Destroy the previous score instance to avoid duplicates
        }
        if (DistanceManager.Instance != null)
        {
            DistanceManager.Instance.SaveDistance();  //Save the distance before starting a new game
            Destroy(DistanceManager.Instance.gameObject);  //Destroy the previous distance instance to avoid duplicates
        }
        SceneManager.LoadScene("Level 1");
    }

    public void QuitGame()
    {
        Score.Instance?.SaveScore();  //Save the score before quitting
        DistanceManager.Instance?.SaveDistance();  //Save the distance before quitting
        Application.Quit();
    }
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void LoadSettingsMenu()
    {
        SceneManager.LoadScene("SettingsScreen");
    }

    public void LoadLeaderboard()
    {
        SceneManager.LoadScene("LeaderBoard");
    }



}
