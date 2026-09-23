using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoadSpawner : MonoBehaviour
{
    //This script manages the spawning and moving of the road prefabs

    [Header("Normal Roads")]
    [Space(10)]
    [SerializeField] private List<GameObject> normalRoads;

    [Tooltip("Optional: collect RoadSpawn trigger parents beneath this root at startup.")]
    [SerializeField] private Transform initialRoadRoot;
    [Tooltip("Only roads on this world X are collected, excluding decorative side streets.")]
    [SerializeField] private float roadCenterX;

    [Header("Construction Roads")]
    [Header("Left:")]
    [SerializeField] private List<GameObject> leftConstrRoads;
    [Header("Right:")]
    [SerializeField] private List<GameObject> rightConstrRoads;

    [Header("Block Roads")]
    [SerializeField] private List<GameObject> blockRoads;

    [Header("Spike Roads")]
    [Header("Left:")]
    [SerializeField] private List<GameObject> leftSpikeRoads;
    [Header("Right:")]
    [SerializeField] private List<GameObject> rightSpikeRoads;
    [Header("Settings")]
    [SerializeField] private float Zoffset = 142f;

    private List<GameObject> currentRoads;
    private ObstacleSpawner obstacleSpawner;
    private SpawnManager spawnManager;
    private bool CanGenerate => currentRoads != null && currentRoads.Count > 0 &&
        (spawnManager == null || spawnManager.GenerationEnabled);
    [HideInInspector]
    public float endConstrZ = 0;
    [HideInInspector]
    public float startConstrZ = 0;


    private void Awake()
    {
        spawnManager = GetComponent<SpawnManager>();
    }

    void Start()
    {
        currentRoads = new List<GameObject>();

        if (initialRoadRoot != null)
        {
            normalRoads = initialRoadRoot.GetComponentsInChildren<Collider>()
                .Where(collider => collider.CompareTag("RoadSpawn") && collider.transform.parent != null)
                .Select(collider => collider.transform.parent.gameObject)
                .Where(road => Mathf.Abs(road.transform.position.x - roadCenterX) < 1f)
                .Distinct().ToList();
        }

        normalRoads = (normalRoads ?? new List<GameObject>()).Where(road => road != null).Distinct().ToList();

        //Order the road lists
        if (normalRoads != null && normalRoads.Count > 0)
        {
            normalRoads = normalRoads.OrderByDescending(r => r.transform.position.z).ToList();
        }
        if (leftConstrRoads != null && leftConstrRoads.Count > 0)
        {
            leftConstrRoads = leftConstrRoads.OrderByDescending(r => r.name).ToList();
        }
        if (rightConstrRoads != null && rightConstrRoads.Count > 0)
        {
            rightConstrRoads = rightConstrRoads.OrderByDescending(r => r.name).ToList();
        }
        //Add the starting roads to the current road list
        for (int i = 0; i < normalRoads.Count; i++)
        {
            currentRoads.Add(normalRoads[i]);
        }

        if (currentRoads.Count == 0)
            Debug.LogError("Assign normal roads or an initial road root to RoadSpawner.", this);

    }

    //This method is just used to get the correct obstacle spawner script
    public void InitializeObsSpawner(ObstacleSpawner obsSpawner)
    {
        obstacleSpawner = obsSpawner;
    }

    //This method moves the normal roads from the current road list to the front
    public void MoveNormalRoad()
    {
        if (!CanGenerate)
            return;
        GameObject movedRoad = currentRoads[0];        //assign the first road which is behind the player by now to a variable

        //Check if the current road prefab has the contruction road marker or the block road marker
        //if it does then destroy that prefab so that we don't move it back to the front 
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
        else
        if (movedRoad.GetComponent<SpikeRoadMarker>())
        {
            currentRoads.RemoveAt(0);
            Destroy(movedRoad);
            movedRoad = currentRoads[0];
        }else
        {
            float newZoffset = currentRoads[currentRoads.Count - 1].transform.position.z - Zoffset;   //get the position for the new road infront of the others

            currentRoads.RemoveAt(0);          //remove the first road from the list
            movedRoad.transform.position = new Vector3(0f, 0f, newZoffset);     //Create a new vector for the new road position
            currentRoads.Add(movedRoad);   //add the new road to the list 
        }
    }

    //This method manages the spawning of the construction roads
    public void SpawnNextConstructionRoad()
    {
        if (!CanGenerate || obstacleSpawner == null)
            return;
        GameObject movedRoad = currentRoads[0];
        float newZoffset = currentRoads[currentRoads.Count - 1].transform.position.z - Zoffset;

        int constrSideChosen = obstacleSpawner.constrSide;
        //Check which lane we need to spawn the new construction road
        List<GameObject> constrSide = DetermineSide(constrSideChosen, leftConstrRoads, rightConstrRoads);
        GameObject constrRoad = null;

        //make sure we don't choose the middle lane
        if (constrSideChosen == 1)
        {
            constrSideChosen = 2;
        }
        obstacleSpawner.laneManager.OccupyLane(constrSideChosen);

        //This switch statement determines which construction road prefab we need to spawn next 
        switch (obstacleSpawner.constrRoadState)
        {
            case ObstacleSpawner.ConstrRoadState.Start:
                constrRoad = constrSide[0];
                obstacleSpawner.constrRoadState = ObstacleSpawner.ConstrRoadState.Middle;
                //keep note of where the construction road starts
                startConstrZ = newZoffset + 80;
                //choose a random number of middle pieces to spawn
                obstacleSpawner.middleConstrRemaining = Random.Range(2, 6);
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
                //Keep note of the position of where the end of the construction road is
                endConstrZ = newZoffset - 80;
                obstacleSpawner.spawningConstrRoad = false;
                break;
        }

        //Spawn the correct construction road prefab for the correct side
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

    //This method manages the spawning of the road block before the boss
    public void SpawnBlockRoad(bool spawnBlockCheck)
    {
        if (!CanGenerate)
            return;
        if (spawnBlockCheck)
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

    //This method manages the spawning of the spike roads
    public void SpawnSpikeRoad()
    {
        if (!CanGenerate || obstacleSpawner == null)
            return;
        if (leftSpikeRoads.Count > 0 && rightSpikeRoads.Count > 0)
        {
            int randomSide = Random.Range(0, 2);
            List<GameObject> spikeRoads = DetermineSide(randomSide, leftSpikeRoads, rightSpikeRoads);
            GameObject movedRoad = currentRoads[0];
            float newZoffset = currentRoads[currentRoads.Count - 1].transform.position.z - Zoffset;
            GameObject spawnedSpikeRoad = Instantiate(spikeRoads[Random.Range(0, spikeRoads.Count)], new Vector3(0f, 0f, newZoffset), Quaternion.Euler(0f, 90f, 0f));
            spawnedSpikeRoad.transform.SetParent(transform, this);
            spawnedSpikeRoad.AddComponent<SpikeRoadMarker>();
            currentRoads.Add(spawnedSpikeRoad);
            obstacleSpawner.spawningSpikeRoad = false;
        }
    }

    //This method makes sure we stay to the correct lane to spawn the construction road
    private List<GameObject> DetermineSide(int randomSide, List<GameObject> leftRoads, List<GameObject> rightRoads)
    {
        switch (randomSide)
        {
            case 0:
                return leftRoads;
            case 1:
                return rightRoads;
            default: return leftRoads;

        }
    }


}
