using System.Collections;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class PauseVolume : MonoBehaviour
{
    [SerializeField] private Volume pauseVolume;
    [Range(-100, 100)]
    [SerializeField] private float saturation = 0f;
    [Range(-10, 10)]
    [SerializeField] private float postExposure = 0f;
    [SerializeField] private float duration = 0.2f;

    private ColorAdjustments colorAdjustments;

    void Start()
    {
        if (pauseVolume.profile.TryGet(out colorAdjustments))
        {
            colorAdjustments.postExposure.overrideState = true;
            colorAdjustments.saturation.overrideState = true;
        }
    }

    public void ApplyPauseEffect()
    {
        colorAdjustments.postExposure.value = postExposure;
        colorAdjustments.saturation.value = saturation;
    }

    public void RemovePauseEffect()
    {
        colorAdjustments.postExposure.value = 0f;
        colorAdjustments.saturation.value = 0f;
    }


}
