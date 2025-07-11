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
    public UIBlock2D PopupBox = null;

    private int selectedIndex = -1;
    private DialoguePopup popupBox;
    private List<Setting> CurrentSettings => SettingsCollection[selectedIndex].Settings;

    private void Start()
    {
        SettingsManager.Instance.LoadAllSettings();

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


        //Popup Box
        popupBox = new DialoguePopup(PopupBox, 0, 0, PopupBox.Size.X.Value, PopupBox.Size.Y.Value, 1f);



    }

    private void OnDisable()
    {
        SettingsManager.Instance.SaveAllSettings();
    }

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

        var sorted = new List<Setting>(CurrentSettings);
        sorted.Sort((a, b) => a.Order.CompareTo(b.Order));

        SettingsList.SetDataSource(sorted);
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
        setting.state = !setting.state;
        target.IsChecked = setting.state;
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
        visuals.IsChecked = setting.state;

        setting.OnStateChanged -= SettingsManager.Instance.UpdateSetting;
        setting.OnStateChanged += SettingsManager.Instance.UpdateSetting;
    }

    private void BindSlider(Data.OnBind<FloatSetting> evt, SliderVisuals visuals, int index)
    {
        FloatSetting setting = evt.UserData;
        visuals.label.Text = setting.Name;
        visuals.ValueLabel.Text = setting.DisplayValue;
        visuals.FillBar.Size.X.Percent = (setting._value - setting.Min) / (setting.Max - setting.Min);

        setting.OnValueChanged -= SettingsManager.Instance.UpdateSetting;
        setting.OnValueChanged += SettingsManager.Instance.UpdateSetting;

    }

    private void BindDropDown(Data.OnBind<MultiOptionSetting> evt, DropDownVisuals visuals, int index)
    {
        MultiOptionSetting setting = evt.UserData;
        visuals.label.Text = setting.Name;
        visuals.SelectedLabel.Text = setting.CurrentSelection;

        setting.OnIndexChanged -= SettingsManager.Instance.UpdateSetting;
        setting.OnIndexChanged += SettingsManager.Instance.UpdateSetting;
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
        visuals.SelectedLabel.Text = setting.Options[setting.selectedIndex];
        visuals.Collapse();
    }

    #endregion

}
