using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
[CreateAssetMenu(menuName = "PowerUp/SpeedPickup")]        //add to the menu to create this object
public class SpeedEffect : PowerUp_Effect
{
    private VignetteController vigController;
    private float intesity = 0.6f;
    private Volume slowDownV;
    public override void ApplyEffect(GameObject target, MonoBehaviour coroutineHost)
    {
        slowDownV = GameObject.FindGameObjectWithTag("SlowVolume").GetComponent<Volume>();
        if (slowDownV != null)
        {
            vigController = new VignetteController(intesity, slowDownV );
            vigController.ApplyVignette(this.duration, coroutineHost);
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
