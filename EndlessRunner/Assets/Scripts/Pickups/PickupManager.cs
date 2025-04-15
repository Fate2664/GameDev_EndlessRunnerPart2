using System.Collections;
using UnityEngine;

public class PickupManager : MonoBehaviour
{
    public static bool PowerUpCheck = false;
    public float duration = 2f;
    private Coroutine activeRoutine;
    public PowerUp_Effect[] powerUps;
    public static string pickup;

    private void Update()
    {
        if (PowerUpCheck && activeRoutine == null)
        {
            activeRoutine = StartCoroutine(PickupRoutine());        //start the coroutine if the powerup is active and it is not already running
        }
    }

    public IEnumerator PickupRoutine()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");     //get the player object
        float setTime = 0f;

        while (setTime < duration)          //make sure that the duration is within the given time
        {
            setTime += Time.deltaTime;
            switch (pickup)                 //check which pickup is being applyed
            {
                case "SpeedPickup":
                    powerUps[0].ApplyEffect(player); break;         //apply the effect onto the player

            }
            yield return null;
        }
        DeactivateEffect(player);          //after the duration disable the effect
        activeRoutine = null;
    }

    public void DeactivateEffect(GameObject player)
    {
        switch (pickup)
        {
            case "SpeedPickup":
                powerUps[0].DisableEffect(player); break;       //Call the diable method onto the player
        }
        PowerUpCheck = false;           //reset the power check to false
    }

    
}
