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
    private PickupOverlayManager pickupOverlayManager;


    private void Start()
    {
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
    
        pickupEffect.ApplyEffect(player, this);
        yield return null;

        powerUpCheck = false;
        activeRoutine = null;
    }


}
