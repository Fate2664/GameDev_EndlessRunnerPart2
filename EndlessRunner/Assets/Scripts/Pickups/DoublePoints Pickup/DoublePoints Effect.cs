using System.Collections;
using TMPro;
using UnityEngine;

[CreateAssetMenu(menuName = "PowerUp/DoublePoints Pickup")]
public class DoublePointsEffect : PowerUp_Effect
{
    //this class inherits from the scriptable object for the pickups and
    //this is where we give the changes that need to be applied when we execute the abstract method
    public override void ApplyEffect(GameObject target, MonoBehaviour coroutineHost)
    {
        coroutineHost.StartCoroutine(PlayEffect(target));
    }

    private IEnumerator PlayEffect(GameObject target)
    {
        float setTime = 0;
        Score Score = FindObjectOfType<Score>();
        if (Score != null)
        {
            Score.DoublePointsActive = true;
        }

        while (setTime < this.duration)
        {
            setTime += Time.deltaTime;
            yield return null;
        }

        Score.DoublePointsActive = false;
    }
}
