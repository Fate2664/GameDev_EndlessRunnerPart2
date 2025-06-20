using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor.Search;

public class PauseScreen : MonoBehaviour
{
    public Button restartButton;
    public Button continueButton;
    public GameObject mainUI;

    [SerializeField] private Volume _volume;
    [Range(-100, 100)]
    [SerializeField] private float _saturation = 0f;
    [Range(-10, 10)]
    [SerializeField] private float _postExposure = 0f;

    private GameObject pauseScreen;
    private bool inPause = false;
    private PauseVolume pauseVolume;

    void Start()
    {
        pauseScreen = this.gameObject;
        pauseScreen.SetActive(false);
        restartButton.onClick.AddListener(RestartGame);
        continueButton.onClick.AddListener(ContinueGame);
        pauseVolume = new PauseVolume(_volume, _saturation, _postExposure);
    }

    private void ShowPauseScreen()
    {
        inPause = true;
        pauseScreen.SetActive(true);
        mainUI.SetActive(false);
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.menuMusic);
        pauseVolume.ApplyPauseEffect();
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
        pauseVolume.RemovePauseEffect();
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
        if (SpawnManager.Instance != null)
        {
            Destroy(SpawnManager.Instance.gameObject);  //Destroy the previous spawn manager instance to avoid duplicates
        }
        SceneManager.LoadScene("Level 1");
    }

    private void ContinueGame()
    {
        Time.timeScale = 1f;
        pauseVolume.RemovePauseEffect();
        mainUI.SetActive(true);
        pauseScreen.SetActive(false);
        inPause = false;
    }
    

}
