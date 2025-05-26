using UnityEngine;

public abstract class PowerUp_Effect : ScriptableObject
{
    //this is the scriptable object for the pickups

    public float duration = 5f;
    public float yHeight = 2f;
    public ParticleSystem particleSystemPrefab;

    //abstract methods for applying and disabling the pickup effect

    public abstract void ApplyEffect(GameObject target, MonoBehaviour coroutineHost);

}
