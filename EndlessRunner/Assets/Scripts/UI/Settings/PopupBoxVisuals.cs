using DG.Tweening;
using Nova;
using NovaSamples.UIControls;
using System;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;


//BUGS:
//When I reopen the popup menu for the second time, the cancel button does not activate for some reason


[System.Serializable]
public class PopupButtonVisuals : ItemVisuals
{
    public TextBlock ButtonLabel = null;
    public UIBlock2D Background = null;

    public Color DefaultColor;
    public Color PressedColor;

}

[System.Serializable]
public class PopupButtonData
{
    public string Label;
    public Action Callback;

    public PopupButtonData(string label, Action callback)
    {
        Label = label;
        Callback = callback;
    }
}

[System.Serializable]
public class PopupBoxVisuals : ItemVisuals
{
    public UIBlock2D Background = null;
    public TextBlock PopupText = null;
    public UIBlock ButtonRoot = null;
    public ListView ButtonList = null;

    public float MaxWidth = 1000;
    public float MaxHeight = 500;
    public float PopinDuration = 0.5f;


    private DialoguePopup Popup;
    private bool EventHandlersRegistered = false;
    private List<PopupButtonData> currentButtons;

    public void Show(string message, List<PopupButtonData> buttons)
    {
        EnsureEventHandlers();
        

        if (Popup == null)
        {
            Popup = new DialoguePopup(Background, 0, 0, MaxWidth, MaxHeight, PopinDuration);

        }

        PopupText.Text = message;
        PopupText.Visible = false;

        currentButtons = buttons;
        Background.gameObject.SetActive(true);
        ButtonList.SetDataSource<PopupButtonData>(null);  
        ButtonList.SetDataSource(currentButtons);
        Popup.PopIn();

        DOTween.Sequence().AppendInterval(0.12f).AppendCallback(() => PopupText.Visible = true);
    }

    public void Hide()
    {
        DOTween.Sequence().AppendInterval(0.3f).AppendCallback(() =>
        {
            PopupText.Visible = false;
            ButtonList.SetDataSource<PopupButtonData>(null);
        });
        
        Popup?.PopOut();
    }

    public void EnsureEventHandlers()
    {
        if (EventHandlersRegistered) return;
        EventHandlersRegistered = true;

        ButtonList.AddGestureHandler<Gesture.OnHover, PopupButtonVisuals>(HandleHover);
        ButtonList.AddGestureHandler<Gesture.OnUnhover, PopupButtonVisuals>(HandleUnhover);
        ButtonList.AddGestureHandler<Gesture.OnPress, PopupButtonVisuals>(HandlePress);
        ButtonList.AddGestureHandler<Gesture.OnRelease, PopupButtonVisuals>(HandleRelease);
        ButtonList.AddGestureHandler<Gesture.OnClick, PopupButtonVisuals>(HandleClick);

        ButtonList.AddDataBinder<PopupButtonData, PopupButtonVisuals>(BindData);
    }

    private void BindData(Data.OnBind<PopupButtonData> evt, PopupButtonVisuals target, int index)
    {
        PopupButtonData data = evt.UserData;
        evt.View.gameObject.SetActive(true);
        target.ButtonLabel.Text = evt.UserData.Label;
    }

    private void HandleClick(Gesture.OnClick evt, PopupButtonVisuals target, int index)
    {
        currentButtons?[index].Callback?.Invoke();
        Hide();
    }

    internal static void HandleHover(Gesture.OnHover evt, PopupButtonVisuals target, int index)
    {
        target.Background.BodyEnabled = true;
        target.Background.Color = target.DefaultColor;
    }

    internal static void HandlePress(Gesture.OnPress evt, PopupButtonVisuals target, int index)
    {
        target.Background.Color = target.PressedColor;
    }

    internal static void HandleRelease(Gesture.OnRelease evt, PopupButtonVisuals target, int index)
    {
        target.Background.Color = target.DefaultColor;
    }

    internal static void HandleUnhover(Gesture.OnUnhover evt, PopupButtonVisuals target, int index)
    {
        target.Background.BodyEnabled = false;
    }

}


