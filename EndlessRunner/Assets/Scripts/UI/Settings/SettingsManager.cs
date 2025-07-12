using Nova;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    public SettingsCollection AudioCollection;
    public SettingsCollection GameplayCollection;
    public SettingsCollection AccessibilityCollection;
    public SettingsCollection VideoCollection;

    private SettingsMenu menu;
    private Dictionary<string, Setting> settingsLookup;

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

    private void Start()
    {
        settingsLookup = new Dictionary<string, Setting>();

        AddSettingsFromCollection(AudioCollection);
        AddSettingsFromCollection(GameplayCollection);
        AddSettingsFromCollection(AccessibilityCollection);
        AddSettingsFromCollection(VideoCollection);
    }


  

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void AddSettingsFromCollection(SettingsCollection collection)
    {
        if (collection == null || collection.Settings == null) return;

        foreach (var setting in collection.Settings)
        {
            if (!string.IsNullOrEmpty(setting.Key))
            {
                if (!settingsLookup.ContainsKey(setting.Key))
                {
                    settingsLookup.Add(setting.Key, setting);
                }
                else
                {
                    Debug.Log("Duplicate setting key found");
                }
            }
        }
    }

    private float GetFloat(string key, float defaultValue)
    {
        if (settingsLookup != null && settingsLookup.TryGetValue(key, out Setting setting))
        {
            if (setting is FloatSetting floatSetting)
                return floatSetting.Value;
        }
        return defaultValue;
    }

    private bool GetBool(string key, bool defaultValue)
    {
        if (settingsLookup != null && settingsLookup.TryGetValue(key, out Setting setting))
        {
            if (setting is BoolSetting boolSetting)
                return boolSetting.State;
        }
        return defaultValue;
    }

    private int GetInt(string key, int defaultValue)
    {
        if (settingsLookup != null && settingsLookup.TryGetValue(key, out Setting setting))
        {
            if (setting is MultiOptionSetting multiOptionSetting)
                return multiOptionSetting.SelectedIndex;
        }
        return defaultValue;
    }

    public bool ParticEnabled => GetBool("ParticlesEnabled", true);
    public int Difficulty => GetInt("Difficulty", 0);
    public float MasterVolume => GetFloat("MasterVolume", 1f);
    public float MusicVolume => GetFloat("Music", 1f);
    public float EffectsVolume => GetFloat("Effects", 1f);
    public float MenuVolume => GetFloat("Menu", 1f);

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "SettingsScreen")
        {
            menu = FindAnyObjectByType<SettingsMenu>();
        }
    }

    #region HandleSettings
    public void ResetAllSettings()
    {
        if (menu == null) return;

        foreach (var collection in menu.SettingsCollection)
        {
            foreach (var setting in collection.Settings)
            {
                setting.ResetToDefault();
            }
        }
        PlayerPrefs.Save();
        menu.SettingsList.Refresh();
    }

    public void LoadAllSettings()
    {
        if (menu == null) return;

        foreach (var collection in menu.SettingsCollection)
        {
            foreach (var setting in collection.Settings)
            {
                switch (setting)
                {
                    case BoolSetting boolSetting: boolSetting.Load(); break;
                    case FloatSetting floatSetting: floatSetting.Load(); break;
                    case MultiOptionSetting multiOptionSetting: multiOptionSetting.Load(); break;
                }
            }
        }
    }

    public void SaveAllSettings()
    {
        if (menu == null) return;

        foreach (var collection in menu.SettingsCollection)
        {
            foreach (var setting in collection.Settings)
            {
                switch (setting)
                {
                    case BoolSetting booleanSetting: booleanSetting?.Save(); break;
                    case FloatSetting floatSetting: floatSetting?.Save(); break;
                    case MultiOptionSetting multiOptionSetting: multiOptionSetting?.Save(); break;
                }
            }
        }
        PlayerPrefs.Save();
    }

    public void UpdateSetting(Setting setting)
    {
        if (setting is FloatSetting floatSetting)
        {
            switch (floatSetting.Type)
            {
                case Setting.SettingType.Audio:
                    AudioManager.Instance.UpdateAllVolumes();
                    break;

            }
        }
        else if (setting is MultiOptionSetting multiOptionSetting)
        {

        }
        else if (setting is BoolSetting boolSetting)
        {

        }

    }
    #endregion

}
