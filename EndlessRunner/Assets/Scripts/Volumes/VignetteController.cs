using Mono.Cecil.Cil;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class VignetteController : MonoBehaviour
{
    //This method controlls the vignette that is applied during a pickup

    [SerializeField] private Volume slowDownVolume;
    [Range(0f, 1f)]
    [SerializeField] private float intensity = 0.5f;
    private Vignette vignette;

    void Start()
    {
        if (slowDownVolume.profile.TryGet(out vignette))
        {
            vignette.intensity.overrideState = true;
        }
    }


    public void ApplyVignette(float duration)
    {
      
        StartCoroutine(FadeVignette(0f, intensity, duration));
    }

    public void RemoveVignette(float duration)
    {
        StartCoroutine(FadeVignette(intensity, 0f, duration));
    }

    //This IENumerator controls the fade in and out of the vignette on the screen 
    private IEnumerator FadeVignette(float start, float end, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            vignette.intensity.value = Mathf.Lerp(start, end, elapsed / duration);
            yield return null;
        }
        vignette.intensity.value = end;
    }
}
