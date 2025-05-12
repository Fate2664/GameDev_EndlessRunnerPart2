using System.Diagnostics.Tracing;
using TMPro;
using UnityEngine;

public class DistanceManager : MonoBehaviour
{
    private float startZ;
    private float actDistance = 0;
    private float _distanceCovered;
    public float distanceCovered { get { return _distanceCovered; } }


    public TextMeshProUGUI distanceValue;

    void Start()
    {
        startZ = transform.position.z;
    }

    void Update()
    {
        actDistance = -(transform.position.z - startZ);
        if (actDistance >= 100)
        {
            _distanceCovered++;
            startZ = transform.position.z;
        }
        if (distanceValue != null)
        {
            distanceValue.text = _distanceCovered.ToString("0");
        }
    }
}
