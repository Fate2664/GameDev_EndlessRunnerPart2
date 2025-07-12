using Nova;
using System;
using UnityEngine;

//this script defines the visuals for a dropdown menu in the settings UI

[System.Serializable]
public class DropDownItemVisuals : ItemVisuals
{
    public TextBlock label = null;
    public UIBlock2D Background = null;
    public UIBlock2D SelectedIndicator = null;

}


[System.Serializable]
public class DropDownVisuals : ItemVisuals
{
    public TextBlock label = null;
    public TextBlock SelectedLabel = null;
    public UIBlock2D Background = null;
    public UIBlock ExpandedRoot = null;
    public ListView OptionsList = null;

    public Color DefaultColor;
    public Color HoveredColor;
    public Color PressedColor;

    public Color PrimaryRowColor;
    public Color SecondaryRowColor;

    private MultiOptionSetting dataSource = null;
    private bool eventHandlersRegistered = false;
    public bool isExpanded => ExpandedRoot.gameObject.activeSelf;

    internal static void HandleHover(Gesture.OnHover evt, DropDownVisuals target)
    {
        if (evt.Receiver.transform.IsChildOf(target.ExpandedRoot.transform))
        {
            return;
        }
        target.Background.Color = target.HoveredColor;
        AudioManager.Instance?.PlaySFX("HoverSound");
    }

    internal static void HandlePress(Gesture.OnPress evt, DropDownVisuals target)
    {
        if (evt.Receiver.transform.IsChildOf(target.ExpandedRoot.transform))
        {
            return;
        }
        target.Background.Color = target.PressedColor;
        AudioManager.Instance?.PlaySFX("ClickSound");
    }

    internal static void HandleRelease(Gesture.OnRelease evt, DropDownVisuals target)
    {
        if (evt.Receiver.transform.IsChildOf(target.ExpandedRoot.transform))
        {
            return;
        }
        target.Background.Color = target.HoveredColor;
    }

    internal static void HandleUnhover(Gesture.OnUnhover evt, DropDownVisuals target)
    {
        if (evt.Receiver.transform.IsChildOf(target.ExpandedRoot.transform))
        {
            return;
        }
        target.Background.Color = target.DefaultColor;
    }

    public void Collapse()
    {
        ExpandedRoot.gameObject.SetActive(false);
    }
    public void Expand(MultiOptionSetting dataSource)
    {
        this.dataSource = dataSource;

        EnsureEventHandlers();

        ExpandedRoot.gameObject.SetActive(true);
        OptionsList.SetDataSource(dataSource.Options);
        OptionsList.JumpToIndex(dataSource.selectedIndex);
    }

    // This method is called when the visuals are initialized.
    private void EnsureEventHandlers()
    {
        if (eventHandlersRegistered)
        {
            return;
        }
        eventHandlersRegistered = true;

        OptionsList.AddGestureHandler<Gesture.OnHover, DropDownItemVisuals>(HandleItemHovered);
        OptionsList.AddGestureHandler<Gesture.OnUnhover, DropDownItemVisuals>(HandleItemUnHovered);
        OptionsList.AddGestureHandler<Gesture.OnPress, DropDownItemVisuals>(HandleItemPressed);
        OptionsList.AddGestureHandler<Gesture.OnRelease, DropDownItemVisuals>(HandleItemReleased);
        OptionsList.AddGestureHandler<Gesture.OnClick, DropDownItemVisuals>(HandleItemClicked);

        OptionsList.AddDataBinder<string, DropDownItemVisuals>(BindItem);
    }

    private void BindItem(Data.OnBind<string> evt, DropDownItemVisuals target, int index)
    {
        target.label.Text = evt.UserData;
        target.SelectedIndicator.gameObject.SetActive(index== dataSource.selectedIndex);
        target.Background.Color = index % 2 == 0 ? PrimaryRowColor : SecondaryRowColor;
    }

    private void HandleItemClicked(Gesture.OnClick evt, DropDownItemVisuals target, int index)
    {
        dataSource.selectedIndex = index;
        SelectedLabel.Text = dataSource.CurrentSelection;
        evt.Consume();
        Collapse();
    }

    private void HandleItemReleased(Gesture.OnRelease evt, DropDownItemVisuals target, int index)
    {
        target.Background.Color = HoveredColor;
    }

    private void HandleItemPressed(Gesture.OnPress evt, DropDownItemVisuals target, int index)
    {
        target.Background.Color = PressedColor;
        AudioManager.Instance?.PlaySFX("ClickSound");
    }

    private void HandleItemUnHovered(Gesture.OnUnhover evt, DropDownItemVisuals target, int index)
    {
        target.Background.Color = index % 2 == 0 ? PrimaryRowColor : SecondaryRowColor;
    }

    private void HandleItemHovered(Gesture.OnHover evt, DropDownItemVisuals target, int index)
    {
        target.Background.Color = HoveredColor;
        AudioManager.Instance?.PlaySFX("HoverSound");
    }
}


