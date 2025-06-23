using Nova;
using UnityEngine;

[System.Serializable]
public class SliderVisuals : ItemVisuals
{
    //this script defines the visuals for the slider in the settings UI
    public TextBlock label = null;
    public UIBlock2D SliderBackground = null;
    public UIBlock2D FillBar = null;
    public TextBlock ValueLabel = null;
}
