using NUnit.Framework.Internal;
using System.Collections;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PickupManager : MonoBehaviour
{
    //This script is used to manage all the pickups and their effects

    [SerializeField] Camera mainCamera;
    [SerializeField] PrometeoCarController prometeoCarController;
    
    [HideInInspector]
    public bool powerUpCheck = false;
    [HideInInspector]
    private PowerUp_Effect _activeEffect;
    public PowerUp_Effect activeEffect { get { return _activeEffect; } }


    private Coroutine activeRoutine;
    private string pickupName;
    private VignetteController vignetteController;
    private PickupOverlayManager pickupOverlayManager;


    private void Start()
    {
        vignetteController = this.GetComponent<VignetteController>();
        pickupOverlayManager = this.GetComponent<PickupOverlayManager>();
    }

    private void Update()
    {
        if (powerUpCheck && activeRoutine == null)
        {
            //start the coroutine if the powerup is active and it is not already running
            activeRoutine = StartCoroutine(PickupRoutine(_activeEffect));
        }

    }

    //use this method to get the corresponding pickup scriptable object
    public void ActivatePickup(PickupLink link)
    {
        if (!powerUpCheck)
        {
            powerUpCheck = true;
            pickupName = link.name;
            _activeEffect = link.powerUp_Effect;
        }
    }

    //This is the IEnumerator that will run while the pickup is active
    public IEnumerator PickupRoutine(PowerUp_Effect link)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PowerUp_Effect pickupEffect = link;
        float setTime = 0f;

        //this is for the hyperdrive effect on the rocket pickup
        if (pickupEffect.particleSystemPrefab != null)
        {
            ParticleSystem pEffect = Instantiate(pickupEffect.particleSystemPrefab, mainCamera.transform.position, Quaternion.identity);
            Vector3 zOffset = mainCamera.transform.forward * -2f;
            pEffect.transform.SetParent(mainCamera.transform);
            pEffect.transform.localPosition = zOffset;
            pEffect.Play();

            if (pEffect != null)
            {
                float duration = pickupEffect.particleSystemPrefab.duration + pickupEffect.particleSystemPrefab.startLifetime;
                float destroyDelay = duration + 10f;
                StartCoroutine(StopPartical(pEffect, duration, destroyDelay));
            }
        }
        //Apply the exhaust flames if the effect has them
        if (pickupEffect.hasTrail)
        {
            prometeoCarController.ExhaustFlamePS();
        }
        //Apply the vignette if the effect has it
        if (pickupEffect.hasVignette)
        {
            vignetteController.ApplyVignette(0.5f);
        }

        //This is to show the pickup overlay when the pickup is active
        switch (pickupName)
        {
            case "Rocket(Clone)":
                pickupOverlayManager.ShowPickupOverlay(PickupOverlayManager.PickupType.Rocket, pickupEffect.duration);
                break;
            case "DoublePoints(Clone)":
                pickupOverlayManager.ShowPickupOverlay(PickupOverlayManager.PickupType.DoublePoints, pickupEffect.duration);
                break;
            case "HourglassUpdated(Clone)":
                pickupOverlayManager.ShowPickupOverlay(PickupOverlayManager.PickupType.HourGlass, pickupEffect.duration);
                break;
        }

        //Apply the effect for the duration of the pickup
        while (setTime < pickupEffect.duration)
        {
            setTime += Time.deltaTime;
            pickupEffect.ApplyEffect(player);
            yield return null;
        }

        if (pickupEffect.hasVignette)
        {
            vignetteController.RemoveVignette(0.5f);
        }
        pickupEffect.DisableEffect(player);
        powerUpCheck = false;
        activeRoutine = null;
    }

    //This IEnumerator is used to stop and then destroy the hyperdrive effect once it is finished
    private IEnumerator StopPartical(ParticleSystem effect, float duration, float destoryDelay)
    {
        yield return new WaitForSeconds(duration);
        if (effect != null)
        {
            effect.Stop();
            Destroy(effect, destoryDelay);
        }
    }


}
