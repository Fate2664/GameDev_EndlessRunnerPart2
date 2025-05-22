using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "PowerUp/RocketPickup")]
public class RocketEffect : PowerUp_Effect
{

    public override void ApplyEffect(GameObject target)
    {
        target.GetComponent<PlayerController>().maxSpeed = 150;
        target.GetComponent<PlayerDeath>().playerImmune = true;
    }

    public override void DisableEffect(GameObject target)
    {
        target.GetComponent<PlayerController>().maxSpeed = 100;
        target.GetComponent<PlayerDeath>().playerImmune = false;
        if (target.GetComponent<PlayerController>().LeftExhaustFlame != null && target.GetComponent<PlayerController>().RightExhaustFlame != null)
        {
            target.GetComponent<PlayerController>().LeftExhaustFlame.Stop();
            target.GetComponent<PlayerController>().RightExhaustFlame.Stop();
        }
    }
}


