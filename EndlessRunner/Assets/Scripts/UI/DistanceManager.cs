using System.Diagnostics.Tracing;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class DistanceManager : MonoBehaviour
{
    //This script manages the distance covered by the player

    private float startZ;
    private float actDistance = 0;
    private float _distanceCovered;
    public float distanceCovered { get { return _distanceCovered; } }


    public List<TextMeshProUGUI> distanceValue;

    void Start()
    {
        startZ = transform.position.z;
    }

    void Update()
    {
        //We get the actual z value that the player covers and increment the distance amount after every 100 units
        actDistance = -(transform.position.z - startZ);
        if (actDistance >= 100)
        {
            _distanceCovered++;
            startZ = transform.position.z;
        }
        if (distanceValue != null)
        {

            for (int i = 0; i < distanceValue.Count; i++)
            {
                distanceValue[i].text = _distanceCovered.ToString("0"); 
            }
            
        }
    }
}
