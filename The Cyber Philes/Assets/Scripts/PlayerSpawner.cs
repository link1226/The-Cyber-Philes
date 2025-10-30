using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class PersistentPlayerSpawner : MonoBehaviour
{
    [Tooltip("Tag of the spawn point object in the scene.")]
    public string spawnTag = "PlayerSpawnPoint";

    void Awake()
    {
        // Keep this manager across scenes
        DontDestroyOnLoad(gameObject);

        // Ensure this runs after a scene is loaded
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Only reposition for a specific scene if desired
        if (scene.buildIndex != 3) return;

        // Find XR Origin
        var xrOrigin = Object.FindFirstObjectByType<XROrigin>();
        if (xrOrigin == null)
        {
            Debug.LogWarning("XR Origin not found in scene.");
            return;
        }

        // Find spawn point
        var spawn = GameObject.FindWithTag(spawnTag);
        if (spawn == null)
        {
            Debug.LogWarning("Spawn point not found in scene.");
            return;
        }

        // Calculate camera offset
        Vector3 cameraOffset = xrOrigin.Camera.transform.position - xrOrigin.transform.position;

        // Move XR Origin so camera is exactly at spawn position
        xrOrigin.transform.position = spawn.transform.position - cameraOffset;

        // Align Y rotation
        Vector3 euler = xrOrigin.transform.eulerAngles;
        euler.y = spawn.transform.eulerAngles.y;
        xrOrigin.transform.eulerAngles = euler;

        Debug.Log($"XR Origin repositioned to spawn point in scene {scene.buildIndex}");
    }
}
