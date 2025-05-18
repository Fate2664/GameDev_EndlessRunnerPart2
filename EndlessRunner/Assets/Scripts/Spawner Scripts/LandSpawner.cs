using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public class LandSpawner : MonoBehaviour
{
    //This script handles the spawning of the plots of land on either side of the road

    private int intialAmount = 15;
    private float landLength = 142f;
    private float xPosLeft = 158.5f;
    private float xPosRight = -158.5f;
    private float previousZ = 160f;
    private float yPos = 1.4f;

    public List<GameObject> plotsofLand;
    private List<GameObject> activePlots = new List<GameObject>();
    void Start()
    {
        //get the intial plots of land and add them to the active list
        GameObject FirstLandLeft = GameObject.Find("BlueHouse_Plot");
        GameObject FirstLandRight = GameObject.Find("BlueHouse_Plot (1)");
        activePlots.Add(FirstLandLeft);
        activePlots.Add(FirstLandRight);
        //call the SpawnLand method for the inital amount
        for (int i = 0; i < intialAmount; i++)
        {
            SpawnLand();
        }

    }


    public void SpawnLand()
    {
        //create a clone from a random plot of land from the list and place it after the previous plot
        GameObject landLeft = Instantiate(plotsofLand[Random.Range(0, plotsofLand.Count)], new Vector3(xPosLeft, yPos, previousZ - landLength), new Quaternion(0, 180, 0, 0));
        GameObject landRight = Instantiate(plotsofLand[Random.Range(0, plotsofLand.Count)], new Vector3(xPosRight, yPos, previousZ - landLength), Quaternion.identity);
        landLeft.transform.SetParent(transform, false);
        landRight.transform.SetParent(transform, false);
        //add them to the active plots list
        activePlots.Add(landLeft);
        activePlots.Add(landRight);
        
        //change the previous z amount to for the new plot to spawn after it
        previousZ -= landLength;
    }

    public void DestroyLand()
    {
        if (activePlots.Count >= 2)
        {
            //get the plots for both side of the road
            GameObject firstPlot = activePlots[0];
            GameObject secondPlot = activePlots[1];
            //destroy those clones
            Destroy(firstPlot);
            Destroy(secondPlot);
            //and remove them from the active list
            activePlots.RemoveAt(0);
            activePlots.RemoveAt(0);
        }
        
    }

}
