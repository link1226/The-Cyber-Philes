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
        GenerateTwoFACode();

        
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

    private float timer = 0;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 15f)
        {
            timer = 0f;
            GenerateTwoFACode();
        }
    }


    // Generate or reset codes here
    public void GenerateTwoFACode()
    {
        twoFACode = Random.Range(100000, 999999).ToString();
        Debug.Log($"New 2FA Code: {twoFACode}");
    }
}
