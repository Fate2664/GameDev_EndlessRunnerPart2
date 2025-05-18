using TMPro;
using UnityEngine;

[CreateAssetMenu(menuName = "PowerUp/DoublePoints Pickup")]
public class DoublePointsEffect : PowerUp_Effect
{
   //this class inherits from the scriptable object for the pickups and
   //this is where we give the changes that need to be applied when we execute the abstract method
    public override void ApplyEffect(GameObject target)
    {
        Score Score = GameObject.FindObjectOfType<Score>();

        if (Score != null)
        {

            Score.DoublePointsActive = true;
        }

    }

    public override void DisableEffect(GameObject target)
    {
        Score Score = GameObject.FindObjectOfType<Score>();

        if (Score != null)
        {

            Score.DoublePointsActive = false;
        }
    }
}
