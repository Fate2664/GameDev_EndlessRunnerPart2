using System.Collections;
using Unity.Collections;
using UnityEngine;

public class PickupManager : MonoBehaviour
{

    [HideInInspector]
    public bool powerUpCheck = false;
  

    private Coroutine activeRoutine;
    private PickupLink _activePickupLink;
    [HideInInspector] 
    public PickupLink activePickupLink { get { return _activePickupLink; } }

    public ParticleSystem RocketParticles;

    private void Awake()
    {
     

        RocketParticles.Stop();
    }


    private void Update()
    {
        if (powerUpCheck && activeRoutine == null)
        {
            activeRoutine = StartCoroutine(PickupRoutine(_activePickupLink));        //start the coroutine if the powerup is active and it is not already running
        }
    }

    public void ActivatePickup(PickupLink link)
    {
        if (!powerUpCheck)
        {
            powerUpCheck = true;
            _activePickupLink = link;
        }
    }
    public IEnumerator PickupRoutine(PickupLink link)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PowerUp_Effect pickupEffect = link.powerUp_Effect;
        float setTime = 0f;

        while (setTime < pickupEffect.duration)
        {
            setTime += Time.deltaTime;
            pickupEffect.ApplyEffect(player, RocketParticles);
            yield return null;
        }

        pickupEffect.DisableEffect(player, RocketParticles);
        powerUpCheck = false;
        activeRoutine = null;
    }

    
}
