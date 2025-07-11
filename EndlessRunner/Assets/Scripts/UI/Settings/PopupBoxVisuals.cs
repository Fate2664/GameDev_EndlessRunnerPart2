using JetBrains.Annotations;
using Nova;
using NovaSamples.UIControls;
using OpenCover.Framework.Model;
using UnityEngine;

[System.Serializable]
public class PopupButtonVisuals : ItemVisuals
{
    public TextBlock ButtonLabel = null;
    public UIBlock2D Background = null;

}

[System.Serializable]
public class PopupBoxVisuals : ItemVisuals
{
    public TextBlock PopupText = null;
    public UIBlock2D Background = null;
    public ListView ButtonList = null;

    public Color DefaultColor;
    public Color SelectedColor;

    public Color DefaultGradientColor;
    public Color HoveredGradientColor;
    public Color PressedGradientColor;


}
