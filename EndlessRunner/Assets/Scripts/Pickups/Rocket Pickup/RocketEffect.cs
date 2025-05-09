using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "PowerUp/RocketPickup")]
public class RocketEffect : PowerUp_Effect
{


    public override void ApplyEffect(GameObject target, ParticleSystem RocketParticles)
    {

        target.GetComponent<PlayerController>().maxSpeed = 150;
        target.GetComponent<PlayerDeath>().PlayerImmune = true;
        RocketParticles.Play();



    }

    public override void DisableEffect(GameObject target, ParticleSystem RocketParticles)
    {

        target.GetComponent<PlayerController>().maxSpeed = 75;
        target.GetComponent<PlayerDeath>().PlayerImmune = false;
        RocketParticles.Stop();

    }
}


