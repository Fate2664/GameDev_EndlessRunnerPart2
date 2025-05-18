using UnityEngine;

public abstract class PowerUp_Effect : ScriptableObject
{
    public float duration = 5f;
    public float yHeight = 2f;
    public ParticleSystem particleSystemPrefab;
    public bool hasTrail = false;
    public bool hasVignette = false;

    //abstract methods for applying and disabling the pickup effect
    public abstract void ApplyEffect(GameObject target);

    public abstract void DisableEffect(GameObject target);
}
