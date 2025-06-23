using Nova;
using System;
using UnityEngine;

[System.Serializable]
public class ToggleVisuals : ItemVisuals
{
    // This script defines the visuals for the toggle button in the settings UI

    public TextBlock label = null;
    public UIBlock2D CheckBox = null;
    public UIBlock2D CheckMark = null;

    public Color DefaultColor;
    public Color HoverColor;
    public Color PressedColor;

    public bool IsChecked
    {
        get => CheckMark.gameObject.activeSelf;
        set => CheckMark.gameObject.SetActive(value);
    }

    internal static void HandleHover(Gesture.OnHover evt, ToggleVisuals target)
    {
        target.CheckBox.Color = target.HoverColor;
        AudioManager.Instance?.PlaySFX("HoverSound");
    }

    internal static void HandlePress(Gesture.OnPress evt, ToggleVisuals target)
    {
        target.CheckBox.Color = target.PressedColor;
        AudioManager.Instance?.PlaySFX("ClickSound");
    }

    internal static void HandleRelease(Gesture.OnRelease evt, ToggleVisuals target)
    {
        target.CheckBox.Color = target.HoverColor;
    }

    internal static void HandleUnhover(Gesture.OnUnhover evt, ToggleVisuals target)
    {
        target.CheckBox.Color = target.DefaultColor;
    }
}


