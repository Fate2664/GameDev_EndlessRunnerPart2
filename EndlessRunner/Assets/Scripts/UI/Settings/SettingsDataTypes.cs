using JetBrains.Annotations;
using System;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
//this script defines various setting types used in a settings menu
public abstract class Setting
{
    public string Key;
    public string Name;

    public enum SettingType
    {
        Audio,
        Video,
        Gameplay,
        Keybinds,
        Accessibility
    }

    public virtual void ResetToDefault() { }
}

[System.Serializable]
public class BoolSetting : Setting  
{
    public bool State;
    public bool DefaultValue = false;
    public SettingType Type;

    public void Save() => PlayerPrefs.SetInt(Key, State ? 1 : 0);
    public void Load() => State = PlayerPrefs.GetInt(Key, DefaultValue ? 1 : 0) == 1;

    public override void ResetToDefault()
    {
        State = DefaultValue;
        Save();
    }
}

[System.Serializable]
public class FloatSetting : Setting
{
    [SerializeField]
    public SettingType Type;
    public float _value;
    public float Min;
    public float Max;
    public string ValueFormat = "{0:0.0}";
    public float DefaultValue = 50f;
    public event Action<float> OnValueChanged;


    public float Value
    {
        get => Mathf.Clamp(_value, Min, Max);
        set
        {
            this._value = Mathf.Clamp(value, Min, Max);
            OnValueChanged?.Invoke(value);
        }
    }

    public string DisplayValue => string.Format(ValueFormat, Value);

    public void Save() => PlayerPrefs.SetFloat(Key, Value);
    public void Load() => Value = PlayerPrefs.GetFloat(Key, DefaultValue);
    public override void ResetToDefault()
    {
        _value = DefaultValue;
        Save();
    }
}

[System.Serializable]
public class MultiOptionSetting : Setting
{
    private const string NothingSelected = "None";
    public SettingType Type;
    public string[] Options = new string[0];
    public int SelectedIndex = 0;
    public int DefaultIndex = 0;

    public string CurrentSelection => SelectedIndex >= 0 && SelectedIndex < Options.Length ? Options[SelectedIndex] : NothingSelected;

    public void Save() => PlayerPrefs.SetInt(Key, SelectedIndex);
    public void Load() => SelectedIndex = PlayerPrefs.GetInt(Key, DefaultIndex);

    public override void ResetToDefault()
    {
        SelectedIndex = DefaultIndex;
        Save();
    }
}

[System.Serializable]
public class ResolutionSetting : MultiOptionSetting
{
    public Resolution[] Resolutions;

    public void Initialize()
    {
        Resolutions = Screen.resolutions;
        Options = new string[Resolutions.Length];
        for (int i = 0; i < Resolutions.Length; i++)
        {
            Resolution r = Screen.resolutions[i];
            Options[i] = $"{r.width} x {r.height} @{r.refreshRateRatio}Hz";
        }
    }

    public Resolution GetSelectedResolution()
    {
        if (Resolutions == null || Resolutions.Length == 0)
        {
            Initialize();
        }
        return Resolutions[Mathf.Clamp(SelectedIndex, 0, Resolutions.Length - 1)];
    }
}