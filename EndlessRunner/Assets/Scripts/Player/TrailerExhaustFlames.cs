using System.Collections;
using UnityEngine;

/// <summary>Plays a timed sequence on the car's existing exhaust particles for trailer shots.</summary>
public class TrailerExhaustFlames : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private bool playOnEnable = true;
    [SerializeField, Min(0f)] private float startDelay = 1f;
    [SerializeField, Min(0.01f)] private float burstDuration = 1f;
    [SerializeField, Min(0.01f)] private float gapBetweenBursts = 1f;
    [SerializeField, Min(2)] private int burstCount = 2;

    private ParticleSystem leftFlame;
    private ParticleSystem rightFlame;
    private Coroutine sequence;

    private void OnEnable()
    {
        if (playOnEnable)
            PlaySequence();
    }

    // Can also be called by a Timeline Signal, UnityEvent or another shot controller.
    public void PlaySequence()
    {
        if (!isActiveAndEnabled)
            return;

        StopSequence();
        if (player == null)
            player = GetComponentInParent<PlayerController>();

        leftFlame = player != null ? player.LeftExhaustFlame : null;
        rightFlame = player != null ? player.RightExhaustFlame : null;
        if (leftFlame == null || rightFlame == null)
        {
            Debug.LogWarning("Assign a player with both exhaust flames to TrailerExhaustFlames.", this);
            return;
        }

        StopFlames(ParticleSystemStopBehavior.StopEmittingAndClear);
        sequence = StartCoroutine(PlayBursts());
    }

    public void StopSequence()
    {
        if (sequence == null)
            return;

        StopCoroutine(sequence);
        sequence = null;
        StopFlames();
    }

    private IEnumerator PlayBursts()
    {
        if (startDelay > 0f)
            yield return new WaitForSeconds(startDelay);

        int count = Mathf.Max(2, burstCount);
        for (int i = 0; i < count; i++)
        {
            if (leftFlame == null || rightFlame == null)
                break;

            leftFlame.Play(true);
            rightFlame.Play(true);
            yield return new WaitForSeconds(Mathf.Max(0.01f, burstDuration));

            StopFlames();
            if (i < count - 1)
                yield return new WaitForSeconds(Mathf.Max(0.01f, gapBetweenBursts));
        }

        StopFlames();
        sequence = null;
    }

    private void StopFlames(ParticleSystemStopBehavior behavior = ParticleSystemStopBehavior.StopEmitting)
    {
        // Let existing particles finish naturally; only clear when restarting a take.
        if (leftFlame != null)
            leftFlame.Stop(true, behavior);
        if (rightFlame != null)
            rightFlame.Stop(true, behavior);
    }

    private void OnDisable()
    {
        StopSequence();
    }
}
