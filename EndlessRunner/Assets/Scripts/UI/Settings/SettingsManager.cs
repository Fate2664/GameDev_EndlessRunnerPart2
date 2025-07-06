using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(Instance);
    }

    public bool ParticEnabled => PlayerPrefs.GetInt("ParticlesEnabled", 1) == 1;
    public float MasterVolume => PlayerPrefs.GetFloat("MasterVolume", 1f);
    public int Difficulty => PlayerPrefs.GetInt("Difficulty", 0);
    public float EffectsVolume => PlayerPrefs.GetFloat("Effects", 1f);
    public float MusicVolume => PlayerPrefs.GetFloat("Music", 1f);
    public float MenuVolume => PlayerPrefs.GetFloat("Menu", 1f);
}
