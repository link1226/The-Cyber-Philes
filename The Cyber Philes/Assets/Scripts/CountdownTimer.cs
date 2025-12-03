using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class CountdownTimer : MonoBehaviour
{

    public static CountdownTimer Instance;
    [Tooltip("Countdown duration in seconds.")]
    public float startSeconds = 300f; // 5 minutes by default

    public NavKeypad.Keypad keypad;

    private float timeRemaining;
    private Label timerLabel;
    private bool timerRunning = true;

    void Awake()
    {
        // Ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        var uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;
        timerLabel = root.Q<Label>("timerLabel");

        timeRemaining = startSeconds;
    }

    void Update()
    {
        if (keypad.accessWasGranted)
            timerRunning = false;

        if (!timerRunning) return;

        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            timerRunning = false;
            OnTimerEnd();
        }

        UpdateTimerDisplay();
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerLabel.text = $"{minutes:00}:{seconds:00}";
    }

    void OnTimerEnd()
    {
        int current = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(current + 2);
    }

    public void SetTimeLeft(float seconds)
    {
        timeRemaining = seconds;
    }

    public void StopTimer()
    {
        timerRunning = false;
    }
}
