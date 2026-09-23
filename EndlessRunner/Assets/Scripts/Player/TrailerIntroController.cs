using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(-100)]
public class TrailerIntroController : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private CinemachineBrain brain;
    [SerializeField] private CinemachineCamera introCamera;
    [SerializeField] private CinemachineCamera gameplayCamera;
    [SerializeField, Min(0f)] private float openingHold = 1f;
    [SerializeField, Min(0f)] private float blendDuration = 6f;

    [Header("HUD")]
    [SerializeField] private CanvasGroup[] hudGroups = Array.Empty<CanvasGroup>();
    [SerializeField, Min(0f)] private float hudFadeDuration = 1f;

    [Header("Driving")]
    [SerializeField] private PlayerController player;
    [Tooltip("Turn off to steer with A/D while recording the camera move.")]
    [SerializeField] private bool scriptedSteering = true;
    [Header("Intro traffic avoidance (world units)")]
    [SerializeField, Min(0.5f)] private float trafficLookAhead = 2f;
    [SerializeField, Min(1f)] private float trafficGap = 20f;
    [SerializeField, Min(0f)] private float trafficSidePadding = 3f;
    [Tooltip("Includes the 0.25-second lane change and a settling margin.")]
    [SerializeField, Min(0.25f)] private float laneChangeSafetyTime = 0.45f;
    [SerializeField, Min(0f)] private float laneChangeCooldown = 0.8f;
    [Tooltip("Deceleration used to calculate a safe following speed, in world units/s squared.")]
    [SerializeField, Min(1f)] private float trafficBraking = 120f;

    [SerializeField] private UnityEvent gameplayCameraReached = new UnityEvent();

    public bool CameraMoveCompleted { get; private set; }

    private bool[] hudInteractable;
    private bool[] hudBlocksRaycasts;
    private TrailerTrafficDriver trafficDriver;

    private void Awake()
    {
        if (brain == null || introCamera == null || gameplayCamera == null || player == null)
        {
            Debug.LogError("Assign the Brain, both cameras and the player to the trailer intro.", this);
            CameraMoveCompleted = true;
            enabled = false;
            return;
        }

        introCamera.Priority = 20;
        gameplayCamera.Priority = 10;
        brain.DefaultBlend = new CinemachineBlendDefinition(
            CinemachineBlendDefinition.Styles.EaseInOut, blendDuration);

        hudInteractable = new bool[hudGroups.Length];
        hudBlocksRaycasts = new bool[hudGroups.Length];
        for (int i = 0; i < hudGroups.Length; i++)
        {
            CanvasGroup group = hudGroups[i];
            if (group == null)
                continue;

            hudInteractable[i] = group.interactable;
            hudBlocksRaycasts[i] = group.blocksRaycasts;
            group.alpha = 0f;
            group.interactable = false;
            group.blocksRaycasts = false;
        }

        // Physics, engine sound and automatic acceleration keep running.
        player.SetCutsceneDriving(true, scriptedSteering);
    }

    private IEnumerator Start()
    {
        if (scriptedSteering)
            trafficDriver = new TrailerTrafficDriver(player);

        // Ensure the Brain has displayed the opening camera, even with a zero hold.
        yield return null;
        if (openingHold > 0f)
            yield return new WaitForSeconds(openingHold);

        gameplayCamera.Priority = 30;
        yield return new WaitUntil(() =>
            brain.ActiveVirtualCamera == (ICinemachineCamera)gameplayCamera && !brain.IsBlending);

        CameraMoveCompleted = true;
        player.SetCutsceneDriving(false);
        gameplayCameraReached.Invoke();

        float elapsed = 0f;
        while (elapsed < hudFadeDuration)
        {
            elapsed += Time.deltaTime;
            SetHudAlpha(Mathf.SmoothStep(0f, 1f, elapsed / hudFadeDuration));
            yield return null;
        }

        SetHudAlpha(1f);
        RestoreHudInput();
    }

    private void FixedUpdate()
    {
        if (!CameraMoveCompleted && scriptedSteering && trafficDriver != null)
            trafficDriver.Tick(trafficLookAhead, trafficGap, trafficSidePadding,
                laneChangeSafetyTime, laneChangeCooldown, trafficBraking);
    }

    private void SetHudAlpha(float alpha)
    {
        foreach (CanvasGroup group in hudGroups)
            if (group != null)
                group.alpha = alpha;
    }

    private void RestoreHudInput()
    {
        if (hudInteractable == null)
            return;

        for (int i = 0; i < hudGroups.Length; i++)
        {
            if (hudGroups[i] == null)
                continue;
            hudGroups[i].interactable = hudInteractable[i];
            hudGroups[i].blocksRaycasts = hudBlocksRaycasts[i];
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        if (player != null)
            player.SetCutsceneDriving(false);
        RestoreHudInput();
    }
}
