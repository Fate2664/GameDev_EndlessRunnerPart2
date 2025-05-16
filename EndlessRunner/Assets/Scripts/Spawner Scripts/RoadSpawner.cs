using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    [Header("Block Roads")]
    [SerializeField] private List<GameObject> blockRoads;

    [Header("Settings")]
    [SerializeField] private float Zoffset = 142f;

    private List<GameObject> currentRoads;
    private ObstacleSpawner obstacleSpawner;
    [HideInInspector]
    public float endConstrZ = 0;
    [HideInInspector]
    public float startConstrZ = 0;
  

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

    public void InitializeObsSpawener(ObstacleSpawner obsSpawner)
    {
        obstacleSpawner = obsSpawner;
    }

    public void MoveNormalRoad()
    {
        GameObject movedRoad = currentRoads[0];        //assign the first road which is behind the player by now to a variable
        while (movedRoad.GetComponent<ConstructionRoadMarker>())
        {
            currentRoads.RemoveAt(0);
            Destroy(movedRoad);
            movedRoad = currentRoads[0];
        }
        if (movedRoad.GetComponent<BlockRoadMarker>())
        {
            currentRoads.RemoveAt(0);
            Destroy(movedRoad);
            movedRoad = currentRoads[0];
        }

        float newZoffset = currentRoads[currentRoads.Count - 1].transform.position.z - Zoffset;   //get the position for the new road infront of the others

        currentRoads.RemoveAt(0);          //remove the first road from the list
        movedRoad.transform.position = new Vector3(0f, 0f, newZoffset);     //Create a new vector for the new road position
        currentRoads.Add(movedRoad);   //add the new road to the list
    }


    public void SpawnNextConstructionRoad()
    {
        GameObject movedRoad = currentRoads[0];
        float newZoffset = currentRoads[currentRoads.Count - 1].transform.position.z - Zoffset;

        int constrSideChosen = obstacleSpawner.constrSide;
        List<GameObject> constrSide = DetermineSide(constrSideChosen);
        GameObject constrRoad = null;

        if (constrSideChosen == 1)
        {
            constrSideChosen = 2;
        }
        obstacleSpawner.laneManager.OccupyLane(constrSideChosen);

        switch (obstacleSpawner.constrRoadState)
        {
            case ObstacleSpawner.ConstrRoadState.Start:
                constrRoad = constrSide[0];
                obstacleSpawner.constrRoadState = ObstacleSpawner.ConstrRoadState.Middle;
                startConstrZ = newZoffset + 80;
                obstacleSpawner.middleConstrRemaining = 4;
                break;
            case ObstacleSpawner.ConstrRoadState.Middle:
                constrRoad = constrSide[1];
                obstacleSpawner.middleConstrRemaining--;

                if (obstacleSpawner.middleConstrRemaining <= 0)
                {
                    obstacleSpawner.constrRoadState = ObstacleSpawner.ConstrRoadState.End;
                }
                break;
            case ObstacleSpawner.ConstrRoadState.End:
                constrRoad = constrSide[2];
                obstacleSpawner.constrRoadState = ObstacleSpawner.ConstrRoadState.None;
                endConstrZ = newZoffset - 80;
                obstacleSpawner.spawningConstrRoad = false;
                break;
        }

        if (constrRoad != null)
        {
            switch (obstacleSpawner.constrSide)
            {
                case 0:
                    movedRoad = Instantiate(constrRoad, new Vector3(0f, 0f, newZoffset), Quaternion.Euler(0f, 90f, 0f));
                    break;
                case 1:
                    movedRoad = Instantiate(constrRoad, new Vector3(0f, 0f, newZoffset), Quaternion.Euler(0f, -90f, 0f));
                    break;
            }
            movedRoad.transform.SetParent(transform, this);
            movedRoad.AddComponent<ConstructionRoadMarker>();
            currentRoads.Add(movedRoad);
        }

    }

    public void SpawnBlockRoad(bool spawnCheck)
    {
        if (spawnCheck)
        {
            GameObject movedRoad = currentRoads[0];
            
            float newZoffset = currentRoads[currentRoads.Count - 1].transform.position.z - Zoffset;
         

            if (blockRoads != null && blockRoads.Count > 0)
            {
                GameObject spawnedBlockRoad = Instantiate(blockRoads[Random.Range(0, blockRoads.Count)], new Vector3(0f, 0f, newZoffset), Quaternion.Euler(0f, 90f, 0f));
                spawnedBlockRoad.transform.SetParent(transform, this);
                spawnedBlockRoad.AddComponent<BlockRoadMarker>();
                currentRoads.Add(spawnedBlockRoad);
            }

        }

    }

    private List<GameObject> DetermineSide(int randomSide)
    {
        switch (randomSide)
        {
            case 0:
                return leftConstrRoads;
            case 1:
                return rightConstrRoads;
            default: return leftConstrRoads;

        }
    }


}
