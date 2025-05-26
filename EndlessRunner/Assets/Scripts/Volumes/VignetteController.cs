using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class VignetteController
{
    //This method controlls the vignette that is applied during a pickup

    private float intensity = 0.6f;
    private Vignette vignette;


    public VignetteController(Volume slowDownVolume, float intensity)
    {
        this.intensity = intensity;
    }


    public void ApplyVignette(float duration, MonoBehaviour coroutineHost)
    {
        coroutineHost.StartCoroutine(FadeVignette(0f, intensity, duration / 2f, () =>
        {
            coroutineHost.StartCoroutine(FadeVignette(intensity, 0f, duration / 2f, null));

        }));
    }

    //This IENumerator controls the fade in and out of the vignette on the screen 
    private IEnumerator FadeVignette(float start, float end, float duration, System.Action onComplete)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            vignette.intensity.value = Mathf.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        vignette.intensity.value = end;

        onComplete?.Invoke();
    }
}
