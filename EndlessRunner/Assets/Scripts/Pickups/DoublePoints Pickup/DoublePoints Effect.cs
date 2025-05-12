using TMPro;
using UnityEngine;

[CreateAssetMenu(menuName = "PowerUp/DoublePoints Pickup")]
public class DoublePointsEffect : PowerUp_Effect
{
   
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
