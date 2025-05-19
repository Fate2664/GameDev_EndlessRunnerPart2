using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseScreen : MonoBehaviour
{
    public Button restartButton;
    public Button continueButton;
    public GameObject mainUI;

    private PauseVolume volume;
    private GameObject pauseScreen;
    private bool inPause = false;
    void Start()
    {
        volume = this.GetComponent<PauseVolume>();
        pauseScreen = this.gameObject;
        pauseScreen.SetActive(false);
        restartButton.onClick.AddListener(RestartGame);
        continueButton.onClick.AddListener(ContinueGame);
    }

    private void ShowPauseScreen()
    {
        inPause = true;
        pauseScreen.SetActive(true);
        mainUI.SetActive(false);
        volume.ApplyPauseEffect();
        Time.timeScale = 0f;

    }

    public void ActivatePauseScreen()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!inPause)
            {
                ShowPauseScreen();
            }else ContinueGame();
        }

    }

    private void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level 1");
        volume.RemovePauseEffect();
    }

    private void ContinueGame()
    {
        volume.RemovePauseEffect();
        Time.timeScale = 1f;
        mainUI.SetActive(true);
        pauseScreen.SetActive(false);
        inPause = false;
    }
}
