using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoadSpawner : MonoBehaviour
{
    public List<GameObject> roads;
    private float Zoffset = 160f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (roads != null && roads.Count > 0)
        {
            roads = roads.OrderByDescending(r => r.transform.position.z).ToList();      //order the road list
        }
    }

    public void MoveRoad()
    {

        GameObject movedRoad = roads[0];        //assign the first road which is behind the player by now to variable
        float newZoffest = roads[roads.Count - 1].transform.position.z - Zoffset;   //get the position for the new road infront of the others
        float newYoffset = 8 + 3.256991f;       
        float newXoffset = -3.64609f;

        roads.RemoveAt(0);          //remove the first road from the list
        movedRoad.transform.position = new Vector3(newXoffset, newYoffset, newZoffest);     //Create a new vector for the new road position
        roads.Add(movedRoad);   //add the new road to the list
    }

   

}
