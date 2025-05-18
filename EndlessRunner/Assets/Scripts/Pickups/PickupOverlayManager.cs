using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PickupOverlayManager : MonoBehaviour
{
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
        rocketPanel.SetActive(false);
        hourGlassPanel.SetActive(false);
        doublePointsPanel.SetActive(false);
    }

    public void ShowPickupOverlay(PickupType type, float duration)
    {
        if (activeTimer !=  null)
        {
            StopCoroutine(activeTimer);
        }

        rocketPanel.SetActive(false);
        hourGlassPanel.SetActive(false);
        doublePointsPanel.SetActive(false);

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
