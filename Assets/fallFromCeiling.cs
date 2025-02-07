using UnityEngine;
using Meta.XR.MRUtilityKit;
using System.Collections.Generic;

public class FallFromCeilingOrFloor : MonoBehaviour
{
    [Tooltip("Reference to the MRUK instance.")]
    [SerializeField]
    private MRUK mrukInstance;

    [Tooltip("Prefab of the cube to spawn.")]
    public GameObject cubePrefab;

    [Tooltip("Time interval in seconds between cube spawns.")]
    public float spawnInterval = 1.0f;

    private bool isCallbackRegistered = false;

    private void OnEnable()
    {
        Debug.Log("FallFromCeilingOrFloor: OnEnable called");

        if (mrukInstance != null && !isCallbackRegistered)
        {
            // Register the callback to be invoked when the scene is fully loaded
            mrukInstance.RegisterSceneLoadedCallback(OnSceneLoaded);
            isCallbackRegistered = true;
        }
        else if (mrukInstance == null)
        {
            Debug.LogError("FallFromCeilingOrFloor: mrukInstance is null. Ensure MRUK is assigned.");
        }
    }

    private void OnDisable()
    {
        Debug.Log("FallFromCeilingOrFloor: OnDisable called");

        // No need to unregister the callback since there's no UnregisterSceneLoadedCallback method
        // The isCallbackRegistered flag prevents multiple registrations
    }

    /// <summary>
    /// Called when the scene has been fully loaded.
    /// </summary>
    private void OnSceneLoaded()
    {
        Debug.Log("FallFromCeilingOrFloor: OnSceneLoaded called");

        // Iterate through all rooms
        foreach (var room in mrukInstance.Rooms)
        {
            // Iterate through all anchors in the room
            foreach (var anchor in room.Anchors)
            {
                Debug.Log($"FallFromCeilingOrFloor: Checking anchor with label {anchor.Label}");

                if (anchor.Label == MRUKAnchor.SceneLabels.CEILING || anchor.Label == MRUKAnchor.SceneLabels.FLOOR)
                {
                    ChangeSurfaceMaterial(anchor);
                }
            }
        }

        // Start spawning cubes
        InvokeRepeating(nameof(SpawnCube), spawnInterval, spawnInterval);
    }

    /// <summary>
    /// Changes the material on the ceiling or floor anchor.
    /// </summary>
    /// <param name="anchor">The ceiling or floor anchor to modify.</param>
    private void ChangeSurfaceMaterial(MRUKAnchor anchor)
    {
        Debug.Log($"FallFromCeilingOrFloor: Detected anchor with label {anchor.Label} and name {anchor.name}");
    }

    private void SpawnCube()
    {
        // Find a random ceiling anchor
        var ceilingAnchors = new List<MRUKAnchor>();
        foreach (var room in mrukInstance.Rooms)
        {
            foreach (var anchor in room.Anchors)
            {
                if (anchor.Label == MRUKAnchor.SceneLabels.CEILING)
                {
                    ceilingAnchors.Add(anchor);
                }
            }
        }

        if (ceilingAnchors.Count == 0)
        {
            Debug.LogWarning("FallFromCeilingOrFloor: No ceiling anchors found.");
            return;
        }

        var randomAnchor = ceilingAnchors[Random.Range(0, ceilingAnchors.Count)];
        var spawnPosition = randomAnchor.transform.position + new Vector3(
            Random.Range(-randomAnchor.transform.localScale.x / 2, randomAnchor.transform.localScale.x / 2),
            0,
            Random.Range(-randomAnchor.transform.localScale.z / 2, randomAnchor.transform.localScale.z / 2)
        );

        var cube = Instantiate(cubePrefab, spawnPosition, Quaternion.identity);
        Destroy(cube, 2.0f); // Destroy the cube after 2 seconds by default

        var cubeRigidbody = cube.GetComponent<Rigidbody>();
        if (cubeRigidbody == null)
        {
            cubeRigidbody = cube.AddComponent<Rigidbody>();
        }

        cube.AddComponent<CubeCollisionHandler>();
    }
}

public class CubeCollisionHandler : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            Destroy(gameObject);
        }
    }
}

