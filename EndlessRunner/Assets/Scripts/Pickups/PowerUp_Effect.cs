using UnityEngine;

public abstract class PowerUp_Effect : ScriptableObject
{
    public float duration = 5f;
    public float yHeight = 2f;

    //abstract methods for applying and disabling the pickup effect
    public abstract void ApplyEffect(GameObject target, ParticleSystem RocketParticles);

    public abstract void DisableEffect(GameObject target, ParticleSystem RocketParticles);
}
