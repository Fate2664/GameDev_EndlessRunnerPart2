using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "PowerUp/RocketPickup")]
public class RocketEffect : PowerUp_Effect
{
    PowerUp_Effect powerUp;


    public override void ApplyEffect(GameObject target)
    {
        powerUp = this.GetComponent<PickupLink>().powerUp_Effect;
        target.GetComponent<PlayerController>().maxSpeed = 150;
        target.GetComponent<PlayerDeath>().PlayerImmune = true;
        if (powerUp.particleSystem != null)
        {
            powerUp.particleSystem.Play();
        }
    }

    public override void DisableEffect(GameObject target)
    {
         powerUp = this.GetComponent<PickupLink>().powerUp_Effect;
        target.GetComponent<PlayerController>().maxSpeed = 100;
        target.GetComponent<PlayerDeath>().PlayerImmune = false;
        if (powerUp.particleSystem != null)
        {
            powerUp.particleSystem.Stop();
        }
    }
}


