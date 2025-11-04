using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Computer Data")]
    public string pcPassword;
    public string webPassword;

    [Header("Shared Game Data")]
    public string phonePasscode;
    public string twoFACode;
    public string doorPasscode;

    void Awake()
    {
        // Ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);  // persists across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Generate or reset codes here
    public void GenerateTwoFACode()
    {
        twoFACode = Random.Range(100000, 999999).ToString();
        Debug.Log($"New 2FA Code: {twoFACode}");
    }
}
