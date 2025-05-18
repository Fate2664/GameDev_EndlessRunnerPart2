using NUnit.Framework.Internal;
using System.Collections;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PickupManager : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] PrometeoCarController prometeoCarController;
    
    [HideInInspector]
    public bool powerUpCheck = false;
    [HideInInspector]
    public PickupLink activePickupLink { get { return _activePickupLink; } }


    private Coroutine activeRoutine;
    private PickupLink _activePickupLink;
    private VignetteController vignetteController;

    private void Start()
    {
        vignetteController = this.GetComponent<VignetteController>();
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

        if (pickupEffect.hasTrail)
        {
            prometeoCarController.ExhaustFlamePS();
        }

        if (pickupEffect.hasVignette)
        {
            vignetteController.ApplyVignette(0.5f);
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
