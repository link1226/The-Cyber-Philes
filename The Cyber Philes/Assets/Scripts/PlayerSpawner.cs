using System.Collections;
using Unity.XR.CoreUtils;
using UnityEngine;

public class SceneSpawnManager : MonoBehaviour
{
    [Tooltip("Tag of the spawn point object in the scene.")]
    public string spawnTag = "PlayerSpawnPoint";

    private IEnumerator Start()
    {
        // Wait one frame to ensure XR Origin and tracking are initialized
        yield return null;

        // Find the XR Origin
        var xrOrigin = FindAnyObjectByType<XROrigin>();
        if (xrOrigin == null)
        {
            Debug.LogWarning("No XROrigin found in scene!");
            yield break;
        }

        // Find the spawn point
        GameObject spawn = GameObject.FindWithTag(spawnTag);
        if (spawn == null)
            spawn = GameObject.Find(spawnTag);

        if (spawn == null)
        {
            Debug.LogWarning($"No {spawnTag} found in scene!");
            yield break;
        }

        // Move XR Origin so the headset aligns with the spawn point
        xrOrigin.MoveCameraToWorldLocation(spawn.transform.position);
        xrOrigin.MatchOriginUpCameraForward(spawn.transform.up, spawn.transform.forward);

        Debug.Log($"XR Origin spawned at {spawn.transform.position}");
    }
}
