using UnityEngine;

[DisallowMultipleComponent]
public class RoadGenerationMarker : MonoBehaviour
{
    [Tooltip("Crossing this road's RoadSpawn trigger allows endless generation to begin.")]
    public bool startEndlessGeneration;
}
