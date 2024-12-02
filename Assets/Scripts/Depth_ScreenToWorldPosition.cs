using Niantic.Lightship.AR.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class Depth_ScreenToWorldPosition : MonoBehaviour
{
    [SerializeField]
    private AROcclusionManager _occlusionManager;

    private XRCpuImage _depthImage;

    private Matrix4x4 _displayMatrix;
    private ScreenOrientation? _latestScreenOrientation;

    [Header("Prefab Spawning")]
    public GameObject prefabToSpawn; // Assign a prefab in the Inspector
    public float spawnInterval = 1f; // Time between spawns
    public float spawnRadius = 5f;  // Radius around the player to spawn prefabs

    private StopwatchUI stopwatchUI; // Reference to the stopwatch
    private Coroutine spawnCoroutine;

    void Start()
    {
        // Find the StopwatchUI component in the scene
        stopwatchUI = FindObjectOfType<StopwatchUI>();

        if (stopwatchUI == null)
        {
            Debug.LogError("StopwatchUI script not found in the scene!");
        }
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
            }
        }
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
            Vector3 spawnPosition = Random.insideUnitSphere * spawnRadius;
            spawnPosition += transform.position;
            spawnPosition.y = transform.position.y; // Adjust for 2D/ground-level spawning

            Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
