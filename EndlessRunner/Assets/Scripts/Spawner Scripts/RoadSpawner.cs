using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class RoadSpawner : MonoBehaviour
{

    [Header("Normal Roads")]
    [Space(10)]
    [SerializeField] private List<GameObject> normalRoads;

    [Header("Construction Roads")]
    [Header("Left:")]
    [SerializeField] private List<GameObject> leftConstrRoads;
    [Header("Right:")]
    [SerializeField] private List<GameObject> rightConstrRoads;

    [Header("Settings")]
    [SerializeField] private float Zoffset = 142f;

    private List<GameObject> currentRoads;
    private ObstacleSpawner obstacleSpawner;


    void Start()
    {
        currentRoads = new List<GameObject>();

        if (normalRoads != null && normalRoads.Count > 0)
        {
            normalRoads = normalRoads.OrderByDescending(r => r.transform.position.z).ToList();      //order the road list
        }
        if (leftConstrRoads != null && leftConstrRoads.Count > 0)
        {
            leftConstrRoads = leftConstrRoads.OrderByDescending(r => r.name).ToList();
        }
        if (rightConstrRoads != null && rightConstrRoads.Count > 0)
        {
            rightConstrRoads = rightConstrRoads.OrderByDescending(r => r.name).ToList();
        }

        for (int i = 0; i < normalRoads.Count; i++)
        {
            currentRoads.Add(normalRoads[i]);
        }

    }

    public void Initialize(ObstacleSpawner obsSpawner)
    {
        obstacleSpawner = obsSpawner;
    }

    public void MoveNormalRoad()
    {
        GameObject movedRoad = currentRoads[0];        //assign the first road which is behind the player by now to a variable
        float newZoffset = currentRoads[currentRoads.Count - 1].transform.position.z - Zoffset;   //get the position for the new road infront of the others

        currentRoads.RemoveAt(0);          //remove the first road from the list
        movedRoad.transform.position = new Vector3(0f, 0f, newZoffset);     //Create a new vector for the new road position
        currentRoads.Add(movedRoad);   //add the new road to the list

    }

    public void SpawnNextConstructionRoad()
    {
        GameObject movedRoad = currentRoads[0];
        float newZoffset = currentRoads[currentRoads.Count - 1].transform.position.z - Zoffset;

        GameObject constrRoad = null;

        switch (obstacleSpawner.constrRoadState)
        {
            case ObstacleSpawner.ConstrRoadState.Start:
                constrRoad = leftConstrRoads[0];
                obstacleSpawner.constrRoadState = ObstacleSpawner.ConstrRoadState.Middle;
                obstacleSpawner.middleConstrRemaining = 4;
                break;
            case ObstacleSpawner.ConstrRoadState.Middle:
                constrRoad = leftConstrRoads[1];
                obstacleSpawner.middleConstrRemaining--;

                if (obstacleSpawner.middleConstrRemaining <= 0)
                {
                    obstacleSpawner.constrRoadState = ObstacleSpawner.ConstrRoadState.End;
                }
                break;
            case ObstacleSpawner.ConstrRoadState.End:
                constrRoad = leftConstrRoads[2];
                obstacleSpawner.constrRoadState = ObstacleSpawner.ConstrRoadState.None;
                obstacleSpawner.spawningConstrRoad = false;
                break;
        }
        
        if (constrRoad != null)
        {
            movedRoad = Instantiate(constrRoad, new Vector3(0f, 0f, newZoffset), Quaternion.Euler(0f, 90f, 0f));
            movedRoad.transform.SetParent(transform, this);
            currentRoads.Add(movedRoad);
        }

        
        currentRoads.RemoveAt(0);
    }

   

}
