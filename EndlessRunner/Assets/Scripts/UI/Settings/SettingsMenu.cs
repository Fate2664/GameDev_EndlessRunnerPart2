using Nova;
using NovaSamples.UIControls;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public UIBlock Root = null;

    public List<SettingsCollection> SettingsCollection = null;
    public ListView TabBar = null;
    public ListView SettingsList = null;

    /*
    [Header("Temporary")]
    public BoolSetting BoolSetting = new BoolSetting();
    public ItemView ToggleItemView = null;
    public FloatSetting FloatSetting = new FloatSetting();
    public ItemView SliderItemView = null;
    public MultiOptionSetting MultiOptionSetting = new MultiOptionSetting();
    public ItemView DropDownItemView = null;
    */

    private int selectedIndex = -1;

    private List<Setting> CurrentSettings => SettingsCollection[selectedIndex].Settings;

    private void Start()
    {
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

        //Temporary
        /*
        BindToggle(BoolSetting, ToggleItemView.Visuals as ToggleVisuals);
        BindSlider(FloatSetting, SliderItemView.Visuals as SliderVisuals);
        BindDropDown(MultiOptionSetting, DropDownItemView.Visuals as DropDownVisuals);
        */

        SettingsList.AddDataBinder<BoolSetting, ToggleVisuals>(BindToggle);
        SettingsList.AddDataBinder<FloatSetting, SliderVisuals>(BindSlider);
        SettingsList.AddDataBinder<MultiOptionSetting, DropDownVisuals>(BindDropDown);

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

    private void HandleSliderDragged(Gesture.OnDrag evt, SliderVisuals target, int index)
    {
        FloatSetting setting = CurrentSettings[index] as FloatSetting;

        Vector3 localPointerPos = target.SliderBackground.transform.InverseTransformPoint(evt.PointerPositions.Current);

        float sliderWidth = target.SliderBackground.CalculatedSize.X.Value;

        float distanceFromLeft = Mathf.Clamp(localPointerPos.x + (sliderWidth * 0.5f), 0f, sliderWidth);

        float percentFromLeft = distanceFromLeft / sliderWidth;

        setting.value = Mathf.Lerp(setting.Min, setting.Max, percentFromLeft);

        target.FillBar.Size.X.Percent = percentFromLeft;
        target.ValueLabel.Text = setting.DisplayValue;
    }

    private void HandleToggleClick(Gesture.OnClick evt, ToggleVisuals target, int index)
    {
        BoolSetting setting = CurrentSettings[index] as BoolSetting;
        setting.State = !setting.State;
        target.IsChecked = setting.State;
    }

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
        visuals.FillBar.Size.X.Percent = (setting.value - setting.Min) / (setting.Max - setting.Min);
    }

    private void BindDropDown(Data.OnBind<MultiOptionSetting> evt, DropDownVisuals visuals, int index)
    {
        MultiOptionSetting setting = evt.UserData;
        visuals.label.Text = setting.Name;
        visuals.SelectedLabel.Text = setting.CurrentSelection;
        visuals.Collapse();
    }
}
