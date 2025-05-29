using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class VignetteController
{
    //This method controlls the vignette that is applied during a pickup

    private float intensity = 0.6f;
    private float fadeDuration = 0.5f;
    private Vignette vignette;

    public VignetteController(float intensity, Volume slowDownV)
    {
        this.intensity = intensity;

        if (slowDownV != null)
        {
            if (slowDownV.profile.TryGet(out vignette))
            {
                vignette.active = true;
                vignette.intensity.overrideState = true;
                vignette.color.overrideState = true;
            }
        }
    }


    public void ApplyVignette(float waitDuration, MonoBehaviour coroutineHost)
    {

        coroutineHost.StartCoroutine(ApplyVignetteRoutine(waitDuration));

    }

    private IEnumerator ApplyVignetteRoutine(float waitDuration)
    {
        yield return FadeVignette(0f, intensity, fadeDuration);

        yield return new WaitForSeconds(waitDuration - .5f);

        yield return FadeVignette(intensity, 0f, fadeDuration);
    }

    //This IENumerator controls the fade in and out of the vignette on the screen 
    private IEnumerator FadeVignette(float start, float end, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            vignette.intensity.value = Mathf.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        vignette.intensity.value = end;

    }
}
