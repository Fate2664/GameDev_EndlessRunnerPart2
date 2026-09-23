using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LandSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> transitionLandPrefabs;
    public List<GameObject> TransitionLandPrefabs => transitionLandPrefabs;
    [SerializeField] private List<GameObject> residentialLandPrefabs;
    public List<GameObject> ResidentialLandPrefabs => residentialLandPrefabs;
    [SerializeField] private List<GameObject> cityLandPrefabs;
    public List<GameObject> CityLandPrefabs => cityLandPrefabs;

    [Range(5, 20)]
    [SerializeField] private int transitionLandCount;
    public int TransitionLandCount => transitionLandCount;

    [Header("Authored Opening")]
    [Tooltip("Register the existing roadside plots instead of generating the initial buffer.")]
    [SerializeField] private bool usePreplacedLayout;
    [Tooltip("Only roadside plots to recycle; exclude distant background buildings.")]
    [SerializeField] private List<Transform> preplacedLeftPlots = new List<Transform>();
    [SerializeField] private List<Transform> preplacedRightPlots = new List<Transform>();

    private const int InitialAmount = 15;
    private const float LandLength = 142f;
    private const float XPosLeft = 158.5f;
    private const float XPosRight = -158.5f;
    private const float YPos = 1.4f;
    private float previousLeftZ = 160f;
    private float previousRightZ = 160f;

    private readonly List<GameObject> activeLeftPlots = new List<GameObject>();
    private readonly List<GameObject> activeRightPlots = new List<GameObject>();
    private SpawnManager spawnManager;
    private bool initialized;
    private bool layoutValid = true;
    private bool initialLandPending;

    private bool CanGenerate => spawnManager == null || spawnManager.GenerationEnabled;

    private void Awake()
    {
        spawnManager = GetComponent<SpawnManager>();
    }

    private void Start()
    {
        InitializeLayout();
        if (CanGenerate && initialLandPending)
            SpawnInitialLand();
    }

    private void InitializeLayout()
    {
        if (initialized)
            return;
        initialized = true;

        if (usePreplacedLayout)
        {
            RegisterPlots(preplacedLeftPlots, activeLeftPlots);
            RegisterPlots(preplacedRightPlots, activeRightPlots);
            layoutValid = activeLeftPlots.Count > 0 && activeRightPlots.Count > 0;
            if (!layoutValid)
            {
                Debug.LogError("Assign preplaced roadside plots on both sides before starting generation.", this);
                return;
            }

            // Use world coordinates and separate cursors for the staggered sides.
            previousLeftZ = activeLeftPlots[activeLeftPlots.Count - 1].transform.position.z;
            previousRightZ = activeRightPlots[activeRightPlots.Count - 1].transform.position.z;
            return;
        }

        // Preserve the existing Level 1 startup layout.
        GameObject firstLeft = GameObject.Find("BlueHouse_Plot");
        GameObject firstRight = GameObject.Find("BlueHouse_Plot (1)");
        if (firstLeft != null)
            activeLeftPlots.Add(firstLeft);
        if (firstRight != null)
            activeRightPlots.Add(firstRight);
        initialLandPending = true;
    }

    private static void RegisterPlots(List<Transform> plots, List<GameObject> active)
    {
        if (plots == null)
            return;
        active.AddRange(plots.Where(plot => plot != null).Distinct()
            .OrderByDescending(plot => plot.position.z).Select(plot => plot.gameObject));
    }

    private void SpawnInitialLand()
    {
        initialLandPending = false;
        for (int i = 0; i < InitialAmount; i++)
            SpawnPair(residentialLandPrefabs);
    }

    public void SpawnLand(List<GameObject> landPrefabs)
    {
        if (!CanGenerate)
            return;
        InitializeLayout();
        if (!layoutValid)
            return;
        if (initialLandPending)
            SpawnInitialLand();
        SpawnPair(landPrefabs);
    }

    private void SpawnPair(List<GameObject> landPrefabs)
    {
        if (landPrefabs == null || landPrefabs.Count == 0)
            return;

        GameObject leftPrefab = landPrefabs[Random.Range(0, landPrefabs.Count)];
        GameObject rightPrefab = landPrefabs[Random.Range(0, landPrefabs.Count)];
        if (leftPrefab == null || rightPrefab == null)
            return;

        GameObject left = Instantiate(leftPrefab,
            new Vector3(XPosLeft, YPos, previousLeftZ - LandLength),
            Quaternion.Euler(0f, 180f, 0f), transform);
        GameObject right = Instantiate(rightPrefab,
            new Vector3(XPosRight, YPos, previousRightZ - LandLength),
            Quaternion.identity, transform);
        activeLeftPlots.Add(left);
        activeRightPlots.Add(right);
        previousLeftZ -= LandLength;
        previousRightZ -= LandLength;
    }

    public void DestroyLand()
    {
        if (!CanGenerate || !layoutValid)
            return;
        DestroyOldest(activeLeftPlots);
        DestroyOldest(activeRightPlots);
    }

    private static void DestroyOldest(List<GameObject> plots)
    {
        if (plots.Count == 0)
            return;
        GameObject oldest = plots[0];
        plots.RemoveAt(0);
        if (oldest != null)
            Destroy(oldest);
    }
}
