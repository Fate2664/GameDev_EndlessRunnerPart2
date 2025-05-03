using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class LaneManager
{
    private readonly float[] lanePositions;
    private bool[] laneOccupied;

    public LaneManager(float[] lanePositions)
    {
        this.lanePositions = lanePositions;
        laneOccupied = new bool[lanePositions.Length];
    }

    //Clears lane at the start of each spawn cycle
    public void ResetLanes()
    {
        for (int i = 0; i < laneOccupied.Length; i++)
            laneOccupied[i] = false;
    }

    //Marks a lane as occupied according to index
    public void OccupyLane(int index)
    {
        if (index >= 0 && index < lanePositions.Length)
            laneOccupied[index] = true;
    }

    //returns the x position for that lane index
    public float GetLaneX(int index)
    {
        return lanePositions[index];
    }

    //returns a random free lane index
    public List<int> GetAllFreeLane()
    {
        List<int> freeLanes = new List<int>();
        for (int i = 0; i < laneCount; i++)
        {
            if (!laneOccupied[i]) freeLanes.Add(i);
        }

        return freeLanes;
    }

    public int laneCount => lanePositions.Length;
}
