using Nova;
using NovaSamples.UIControls;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    //This script is used to create a settings menu with tabs, toggles, sliders, and dropdowns.

    public UIBlock Root = null;

    public List<SettingsCollection> SettingsCollection = null;
    public ListView TabBar = null;
    public ListView SettingsList = null;

    private int selectedIndex = -1;

    private List<Setting> CurrentSettings => SettingsCollection[selectedIndex].Settings;

    private void Start()
    {
        LoadAllSettings();
        //ResetAllSettings();

        //Visual
        Root.AddGestureHandler<Gesture.OnHover, ToggleVisuals>(ToggleVisuals.HandleHover);
        Root.AddGestureHandler<Gesture.OnUnhover, ToggleVisuals>(ToggleVisuals.HandleUnhover);
        Root.AddGestureHandler<Gesture.OnPress, ToggleVisuals>(ToggleVisuals.HandlePress);
        Root.AddGestureHandler<Gesture.OnRelease, ToggleVisuals>(ToggleVisuals.HandleRelease);

        Root.AddGestureHandler<Gesture.OnHover, DropDownVisuals>(DropDownVisuals.HandleHover);
        Root.AddGestureHandler<Gesture.OnUnhover, DropDownVisuals>(DropDownVisuals.HandleUnhover);
        Root.AddGestureHandler<Gesture.OnPress, DropDownVisuals>(DropDownVisuals.HandlePress);
        Root.AddGestureHandler<Gesture.OnRelease, DropDownVisuals>(DropDownVisuals.HandleRelease);

        //State Changing
        SettingsList.AddGestureHandler<Gesture.OnClick, ToggleVisuals>(HandleToggleClick);
        SettingsList.AddGestureHandler<Gesture.OnDrag, SliderVisuals>(HandleSliderDragged);
        SettingsList.AddGestureHandler<Gesture.OnClick, DropDownVisuals>(HandleDropDownClick);

        //Data Binding
        SettingsList.AddDataBinder<BoolSetting, ToggleVisuals>(BindToggle);
        SettingsList.AddDataBinder<FloatSetting, SliderVisuals>(BindSlider);
        SettingsList.AddDataBinder<MultiOptionSetting, DropDownVisuals>(BindDropDown);
        SettingsList.AddDataBinder<ResolutionSetting, DropDownVisuals>(BindResolution);

        //Tabs
        TabBar.AddDataBinder<SettingsCollection, TabButtonVisuals>(BindTab);
        TabBar.AddGestureHandler<Gesture.OnHover, TabButtonVisuals>(TabButtonVisuals.HandleHover);
        TabBar.AddGestureHandler<Gesture.OnPress, TabButtonVisuals>(TabButtonVisuals.HandelPress);
        TabBar.AddGestureHandler<Gesture.OnUnhover, TabButtonVisuals>(TabButtonVisuals.HandleUnHover);
        TabBar.AddGestureHandler<Gesture.OnRelease, TabButtonVisuals>(TabButtonVisuals.HandleRelease);
        TabBar.AddGestureHandler<Gesture.OnClick, TabButtonVisuals>(HandleTabClicked);

        TabBar.SetDataSource(SettingsCollection);

        if (TabBar.TryGetItemView(0, out ItemView firstTab))
        {
            SelectTab(firstTab.Visuals as TabButtonVisuals, 0);
        }

    }


    private void OnDisable()
    {
        SaveAllSettings();
    }

    //This method is used to select a tab and update the settings list accordingly.
    #region HandleData
    private void SelectTab(TabButtonVisuals visuals, int index)
    {
        if (index == selectedIndex)
        {
            return;
        }

        if (selectedIndex >= 0 && TabBar.TryGetItemView(selectedIndex, out ItemView currentItemView))
        {
            (currentItemView.Visuals as TabButtonVisuals).isSelected = false;
        }

        selectedIndex = index;
        visuals.isSelected = true;
        SettingsList.SetDataSource(CurrentSettings);
    }

    private void HandleTabClicked(Gesture.OnClick evt, TabButtonVisuals target, int index)
    {
        SelectTab(target, index);
    }

    private void HandleDropDownClick(Gesture.OnClick evt, DropDownVisuals target, int index)
    {
        MultiOptionSetting setting = CurrentSettings[index] as MultiOptionSetting;

        if (target.isExpanded)
        {
            target.Collapse();
        }
        else
        {
            target.Expand(setting);
        }
    }

    // This method is called when the slider is dragged. It updates the value of the setting based on the position of the pointer.
    private void HandleSliderDragged(Gesture.OnDrag evt, SliderVisuals target, int index)
    {

        FloatSetting setting = CurrentSettings[index] as FloatSetting;

        Vector3 localPointerPos = target.SliderBackground.transform.InverseTransformPoint(evt.PointerPositions.Current);

        float sliderWidth = target.SliderBackground.CalculatedSize.X.Value;

        float distanceFromLeft = Mathf.Clamp(localPointerPos.x + (sliderWidth * 0.5f), 0f, sliderWidth);

        float percentFromLeft = distanceFromLeft / sliderWidth;

        setting.Value = Mathf.Lerp(setting.Min, setting.Max, percentFromLeft);

        target.FillBar.Size.X.Percent = percentFromLeft;
        target.ValueLabel.Text = setting.DisplayValue;
    }

    private void HandleToggleClick(Gesture.OnClick evt, ToggleVisuals target, int index)
    {
        BoolSetting setting = CurrentSettings[index] as BoolSetting;
        setting.State = !setting.State;
        target.IsChecked = setting.State;
    }

    #endregion

    //These methods are used to bind the data to the visuals for each type of setting.
    #region BindData
    private void BindTab(Data.OnBind<SettingsCollection> evt, TabButtonVisuals target, int index)
    {
        target.label.Text = evt.UserData.Category;
        target.isSelected = false;
    }

    private void BindToggle(Data.OnBind<BoolSetting> evt, ToggleVisuals visuals, int index)
    {
        BoolSetting setting = evt.UserData;
        visuals.label.Text = setting.Name;
        visuals.IsChecked = setting.State;
    }

    private void BindSlider(Data.OnBind<FloatSetting> evt, SliderVisuals visuals, int index)
    {
        FloatSetting setting = evt.UserData;
        visuals.label.Text = setting.Name;
        visuals.ValueLabel.Text = setting.DisplayValue;
        visuals.FillBar.Size.X.Percent = (setting._value - setting.Min) / (setting.Max - setting.Min);

        if (setting != null && setting.Type == Setting.SettingType.Audio)
        {
            setting.OnValueChanged -= OnAudioSettingChange;
            setting.OnValueChanged += OnAudioSettingChange;
        }

    }

    private void OnAudioSettingChange(float value)
    {
        AudioManager.Instance.UpdateAllVolumes();
    }

    private void BindDropDown(Data.OnBind<MultiOptionSetting> evt, DropDownVisuals visuals, int index)
    {
        MultiOptionSetting setting = evt.UserData;
        visuals.label.Text = setting.Name;
        visuals.SelectedLabel.Text = setting.CurrentSelection;
        visuals.Collapse();
    }

    private void BindResolution(Data.OnBind<ResolutionSetting> evt, DropDownVisuals visuals, int index)
    {
        ResolutionSetting setting = evt.UserData;

        if (setting.Options == null || setting.Options.Length == 0)
        {
            setting.Initialize();
        }

        visuals.label.Text = setting.Name;
        visuals.SelectedLabel.Text = setting.Options[setting.SelectedIndex];
        visuals.Collapse();
    }

    #endregion


    #region HandleSettings
    private void ResetAllSettings()
    {
        foreach (var collection in SettingsCollection)
        {
            foreach (var setting in collection.Settings)
            {
                setting.ResetToDefault();
            }
        }
        PlayerPrefs.Save();
    }

    private void LoadAllSettings()
    {
        foreach (var collection in SettingsCollection)
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

    private void SaveAllSettings()
    {
        foreach (var collection in SettingsCollection)
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
    #endregion

}
