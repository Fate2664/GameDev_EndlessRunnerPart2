using NUnit.Framework.Internal;
using System;
using System.Collections;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

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

    [Header("Pickup Events")]
    public UnityEvent<PowerUp_Effect> OnRocketPickup;
    public UnityEvent<PowerUp_Effect> OnDoublePointsPickup;
    public UnityEvent<PowerUp_Effect> OnHourglassPickup;


    private Coroutine activeRoutine;
    private string pickupName;
    private PickupOverlayManager pickupOverlayManager;


    private void Start()
    {
        pickupOverlayManager = this.GetComponent<PickupOverlayManager>();

        // Initialize the pickup events if they are not already set
        OnRocketPickup ??= new UnityEvent<PowerUp_Effect>();
        OnDoublePointsPickup ??= new UnityEvent<PowerUp_Effect>();
        OnHourglassPickup ??= new UnityEvent<PowerUp_Effect>();

        // Subscribe to the pickup events
        OnRocketPickup.AddListener(HandleRocketPickup);
        OnDoublePointsPickup.AddListener(HandleDoublePointsPickup);
        OnHourglassPickup.AddListener(HandleHourglassPickup);
    }

    private void HandleDoublePointsPickup(PowerUp_Effect effect)
    {
        ActivatePickup(effect, "DoublePoints(Clone)", PickupOverlayManager.PickupType.DoublePoints);
    }

    private void HandleHourglassPickup(PowerUp_Effect effect)
    {
        ActivatePickup(effect, "HourglassUpdated(Clone)", PickupOverlayManager.PickupType.HourGlass);
    }

    private void HandleRocketPickup(PowerUp_Effect effect)
    {
        ActivatePickup(effect, "Rocket(Clone)", PickupOverlayManager.PickupType.Rocket);
    }

    private void Update()
    {
        if (powerUpCheck && activeRoutine == null)
        {
            //start the coroutine if the powerup is active and it is not already running
            activeRoutine = StartCoroutine(PickupRoutine(_activeEffect));
        }

    }

    //use this method to get the corresponding pickup scriptable objects
    public void ActivatePickup(PowerUp_Effect effect, string name, PickupOverlayManager.PickupType type)
    {
        if (!powerUpCheck)
        {
            powerUpCheck = true;
            pickupName = name;
            _activeEffect = effect;
        }

        pickupOverlayManager.ShowPickupOverlay(type, effect.duration);

        if (activeRoutine == null)
        {
            activeRoutine = StartCoroutine(PickupRoutine(_activeEffect));
        }
    }

    //This is the IEnumerator that will run while the pickup is active
    public IEnumerator PickupRoutine(PowerUp_Effect effect)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        effect.ApplyEffect(player, this);
        yield return null;

        powerUpCheck = false;
        activeRoutine = null;
    }


}
