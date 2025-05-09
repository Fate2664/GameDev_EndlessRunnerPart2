using UnityEngine;
[CreateAssetMenu (menuName = "PowerUp/SpeedPickup")]        //add to the menu to create this object
public class SpeedEffect : PowerUp_Effect
{
    
    public override void ApplyEffect(GameObject target, ParticleSystem RocketParticles)
    {
        target.GetComponent<PlayerController>().maxSpeed = 50;       //change the player's movement speed to the given value
    }

    public override void DisableEffect(GameObject target, ParticleSystem RocketParticles)
    {
        target.GetComponent<PlayerController>().maxSpeed = 100;         //change the player's movemennt speed back to the original
    }

}
