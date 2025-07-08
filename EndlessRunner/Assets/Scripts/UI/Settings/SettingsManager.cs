using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    public SettingsCollection AudioCollection;
    public SettingsCollection GameplayCollection;
    public SettingsCollection AccessibilityCollection;
    public SettingsCollection VideoCollection;

    private FloatSetting MasterVolumeSetting;
    private FloatSetting MusicVolumeSetting;
    private FloatSetting EffectsVolumeSetting;
    private FloatSetting MenuVolumeSetting;

    private void Start()
    {
        AudioCollection.Settings.ForEach(setting =>
        {
            if (setting.Key == "MasterVolume")
            {
                MasterVolumeSetting = (FloatSetting)setting;
            }
            else if (setting.Key == "Music")
            {
                MusicVolumeSetting = (FloatSetting)setting;
            }
            else if (setting.Key == "Effects")
            {
                EffectsVolumeSetting = (FloatSetting)setting;
            }
            else if (setting.Key == "Menu")
            {
                MenuVolumeSetting = (FloatSetting)setting;
            }
        });
    }
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
    public int Difficulty => PlayerPrefs.GetInt("Difficulty", 0);

    public float MasterVolume => MasterVolumeSetting?.Value ?? 1f;
    public float MusicVolume => MusicVolumeSetting?.Value ?? 1f;
    public float EffectsVolume => EffectsVolumeSetting?.Value ?? 1f;
    public float MenuVolume => MenuVolumeSetting?.Value ?? 1f;

}
