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
        SceneManager.LoadScene("Level 1");
        pauseVolume.RemovePauseEffect();
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
