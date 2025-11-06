using UnityEngine;
using UnityEngine.SceneManagement;

public class WinSceneTrigger : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;

    private void OnTriggerEnter(Collider other)
    {
        // Detect XR player (the camera rig or its colliders)
        if (other.CompareTag("Player") || other.name.Contains("XR Origin"))
        {
            // Optional: prevent multiple loads
            if (!SceneManager.GetSceneByName(sceneToLoad).isLoaded)
                SceneManager.LoadScene(sceneToLoad);
        }
    }
}
