using UnityEngine;

public abstract class PowerUp_Effect : ScriptableObject
{
    //this is the scriptable object for the pickups

    public float duration = 5f;
    public float yHeight = 2f;
    //there is probably a better way to do this.....
    //I will maybe change it in Part 3
    public ParticleSystem particleSystemPrefab;
    public bool hasTrail = false;
    public bool hasVignette = false;

    //abstract methods for applying and disabling the pickup effect
    public abstract void ApplyEffect(GameObject target);

    public abstract void DisableEffect(GameObject target);
}
