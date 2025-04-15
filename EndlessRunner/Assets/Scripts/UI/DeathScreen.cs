using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathScreen : MonoBehaviour
{

    public GameObject deathScreen;
    public Button restartButton;
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        deathScreen.SetActive(false);   //Don't show the death screen when game begins
        restartButton.onClick.AddListener(RestartGame);     //When clicked on the restart button call the RestartGame method
    }

    public void ShowDeathScreen()
    {
      
      deathScreen.SetActive(true);      //Show the death screen
        Time.timeScale = 0f;            //Freeze the game

    }

    private void RestartGame()
    {
        Time.timeScale = 1f;            //reset the game timer
        PickupManager.PowerUpCheck = false;     //reset the pickup power up
        SceneManager.LoadScene("Level 1");      //reload the level

    }


}
