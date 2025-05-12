using NUnit.Framework.Internal;
using System.Collections;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PickupManager : MonoBehaviour
{
    [SerializeField] Camera mainCamera;

    [HideInInspector]
    public bool powerUpCheck = false;
    private Coroutine activeRoutine;
    private PickupLink _activePickupLink;
    [HideInInspector]
    public PickupLink activePickupLink { get { return _activePickupLink; } }


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
                Destroy(pEffect, pickupEffect.particleSystemPrefab.duration + pickupEffect.particleSystemPrefab.startLifetime);
            }

        }

        while (setTime < pickupEffect.duration)
        {
            setTime += Time.deltaTime;
            pickupEffect.ApplyEffect(player);
            yield return null;
        }
        pickupEffect.DisableEffect(player);
        powerUpCheck = false;
        activeRoutine = null;
    }


}
