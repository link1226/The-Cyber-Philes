using UnityEngine;

public class LevelManager2 : MonoBehaviour
{
    public static LevelManager2 Instance;

    [Header("Computer Data")]
    public string pcPassword;
    public string webPassword;
    public string twoFACode;

    public bool softwareUpdated = false;
    public bool deviceBlacklisted = false;
    public bool mariePasswordReset = false;

    public string inboxStage = "2250";

    void Awake()
    {
        GeneratePCPassword();
        // GenerateTwoFACode();

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

    public void GeneratePCPassword()
    {
        pcPassword = Random.Range(100000, 999999).ToString();
    }

    public void GenerateTwoFACode()
    {
        twoFACode = Random.Range(100000, 999999).ToString();
    }
}