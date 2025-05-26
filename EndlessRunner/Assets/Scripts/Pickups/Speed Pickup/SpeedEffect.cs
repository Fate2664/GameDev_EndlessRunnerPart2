using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
[CreateAssetMenu(menuName = "PowerUp/SpeedPickup")]        //add to the menu to create this object
public class SpeedEffect : PowerUp_Effect
{
    private VignetteController vigController;
    private Volume vigVolume;
    private float vigDuration = 0.1f;
    private float intesity = 0.6f;
    private Vignette vignette;

    public override void ApplyEffect(GameObject target, MonoBehaviour coroutineHost)
    {

        vigVolume = GameObject.FindWithTag("PostProcessing").GetComponent<Volume>();
        if (vigVolume != null)
        {
            vigController = new VignetteController(vigVolume, intesity);
            if (vigVolume.profile.TryGet(out vignette))
            {
                vignette.intensity.overrideState = true;
            }
            vigController.ApplyVignette(vigDuration, coroutineHost);
        }
        coroutineHost.StartCoroutine(PlayEffect(target));
    }


    private IEnumerator PlayEffect(GameObject target)
    {
        float setTime = 0f;
        target.GetComponent<PlayerController>().maxSpeed = 50;       //change the player's movement speed to the given value 

        while (setTime < this.duration)
        {
            setTime += Time.deltaTime;
            yield return null;
        }

        target.GetComponent<PlayerController>().maxSpeed = 100;         //change the player's movemennt speed back to the original
    }


}
