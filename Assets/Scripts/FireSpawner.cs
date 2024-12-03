using Niantic.Lightship.AR.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class FireSpawner : MonoBehaviour
{
    [SerializeField]
    private AROcclusionManager _occlusionManager;

    private XRCpuImage _depthImage;

    private Matrix4x4 _displayMatrix;
    private ScreenOrientation? _latestScreenOrientation;

    [Header("Prefab Spawning")]
    public GameObject prefabToSpawn; // Assign a prefab in the Inspector
    public float floorOffset;
    public float spawnInterval = 1f; // Time between spawns
    public float minSpawnRadius = 5f;  // Radius around the player to spawn prefabs
    public float maxSpawnRadius = 5f;  // Radius around the player to spawn prefabs
    public int maxFireCount;
    public LayerMask spawnMask;
    
    [SerializeField] private List<GameObject> pooledFires = new List<GameObject>();
    [SerializeField] private List<GameObject> spawnedFires = new List<GameObject>();

    private GameUI stopwatchUI; // Reference to the stopwatch
    private Coroutine spawnCoroutine;

    void Start()
    {
        // Find the StopwatchUI component in the scene
        stopwatchUI = FindObjectOfType<GameUI>();

        if (stopwatchUI == null)
        {
            Debug.LogError("StopwatchUI script not found in the scene!");
        }

        if(this._occlusionManager == null){
            this._occlusionManager = GameObject.Find("Main Camera").GetComponent<AROcclusionManager>();
        }

        PoolFires();
    }

    private void OnDestroy() {
        TurnOffAllFires();

        foreach(GameObject fire in pooledFires){
            Destroy(fire);
        }

        pooledFires.Clear();
    }

    void Update()
    {
        UpdateImage();
        UpdateDisplayMatrix();

        // Start or stop spawning based on the stopwatch state
        if (stopwatchUI != null)
        {

            if (stopwatchUI.IsRunning() && spawnCoroutine == null)
            {
                spawnCoroutine = StartCoroutine(SpawnPrefabs());
            }
            else if (!stopwatchUI.IsRunning() && spawnCoroutine != null)
            {
                StopCoroutine(spawnCoroutine);
                spawnCoroutine = null;
                TurnOffAllFires();
            }
        }
    }

    public void TurnOffAllFires(){
        foreach(GameObject fire in spawnedFires){
            fire.SetActive(false);
            pooledFires.Add(fire);
        }

        spawnedFires.Clear();
    }

    private void PoolFires(){
        for(int i = 0; i < maxFireCount; i++){
            GameObject fire = Instantiate(prefabToSpawn, Vector3.zero, Quaternion.identity);
            fire.SetActive(false);
            this.pooledFires.Add(fire);
        }
    }

    private void SpawnFireAt(Vector3 position){
        GameObject fire = this.pooledFires[this.pooledFires.Count - 1];

        this.pooledFires.Remove(fire);
        this.spawnedFires.Add(fire);

        fire.transform.position = position;
        fire.SetActive(true);
    }

    private void UpdateImage()
    {
        if (!_occlusionManager.subsystem.running)
        {
            return;
        }

        if (_occlusionManager.TryAcquireEnvironmentDepthCpuImage(out var image))
        {
            _depthImage.Dispose();
            _depthImage = image;
        }
    }

    private void UpdateDisplayMatrix()
    {
        if (_depthImage is { valid: true })
        {
            if (!_latestScreenOrientation.HasValue ||
                _latestScreenOrientation.Value != XRDisplayContext.GetScreenOrientation())
            {
                _latestScreenOrientation = XRDisplayContext.GetScreenOrientation();
                _displayMatrix = CameraMath.CalculateDisplayMatrix(
                    _depthImage.width,
                    _depthImage.height,
                    Screen.width,
                    Screen.height,
                    _latestScreenOrientation.Value,
                    invertVertically: true);
            }
        }
    }

    private IEnumerator SpawnPrefabs()
    {
        while (true)
        {
            Debug.Log("Attempting Spawn");
            float randomSpawnDist = Random.Range(minSpawnRadius, maxSpawnRadius);
            if(Physics.Raycast(this.transform.position, Random.onUnitSphere, out var hit, maxSpawnRadius, spawnMask)){
                // Debug.Log(hit.collider.gameObject + " Distance: " + hit.distance);
                
                if(hit.collider.gameObject.GetComponent<ARPlane>() == null){
                    yield return new WaitForEndOfFrame();
                    continue;
                }
                if(hit.distance < minSpawnRadius) {
                    yield return new WaitForEndOfFrame();
                    continue;
                }


                Vector3 spawnPosition = hit.point;
                this.SpawnFireAt(spawnPosition);

                yield return new WaitForSeconds(spawnInterval);
            }
            yield return new WaitForEndOfFrame();
        }
    }
}
