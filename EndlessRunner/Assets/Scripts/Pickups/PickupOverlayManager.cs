using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PickupOverlayManager : MonoBehaviour
{
    //This script manages the pickup overlays for when the pickup is active
    [SerializeField] private Canvas pickupOverlay;

    [Header("Pickup Panels")]
    [SerializeField] private GameObject rocketPanel;
    [SerializeField] private GameObject hourGlassPanel;
    [SerializeField] private GameObject doublePointsPanel;

    [Header("Circle Timers")]
    [SerializeField] private Image rocketCircleTimer;
    [SerializeField] private Image hourGlassCircleTimer;
    [SerializeField] private Image doublePointsCircleTimer;

    private Coroutine activeTimer;

    public enum PickupType
    {
        Rocket,
        HourGlass,
        DoublePoints
    }
    void Start()
    {
        //set the pickup overlays to be disabled
        rocketPanel.SetActive(false);
        hourGlassPanel.SetActive(false);
        doublePointsPanel.SetActive(false);
    }

    public void ShowPickupOverlay(PickupType type, float duration)
    {
        //stops the coroutine if the coroutine is null
        if (activeTimer !=  null)
        {
            StopCoroutine(activeTimer);
        }

        rocketPanel.SetActive(false);
        hourGlassPanel.SetActive(false);
        doublePointsPanel.SetActive(false);

        //switch statement to check which pickup is active and enable the panel with the circle timer coroutine 
        switch (type)
        {
            case PickupType.Rocket:
                rocketPanel.SetActive(true);
                activeTimer = StartCoroutine(RunCircleTimer(rocketCircleTimer, rocketPanel, duration));
                break;
            case PickupType.HourGlass:
                hourGlassPanel.SetActive(true);
                activeTimer = StartCoroutine(RunCircleTimer(hourGlassCircleTimer, hourGlassPanel, duration));
                break;
            case PickupType.DoublePoints:
                doublePointsPanel.SetActive(true);
                activeTimer = StartCoroutine(RunCircleTimer(doublePointsCircleTimer, doublePointsPanel, duration));
                break;
        }
    }

    //This IEnumerator will run down the circle timer until the end of the pickup duration 
    private IEnumerator RunCircleTimer(Image timerImage, GameObject panel , float duration)
    {
        float time = 0f;
        timerImage.fillAmount = 1f;

        while (time < duration)
        {
            time += Time.deltaTime;
            timerImage.fillAmount = 1f - (time / duration);
            yield return null;
        }

        timerImage.fillAmount = 0f;
        panel.SetActive(false);
        activeTimer = null;
    }
}
