using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class WinLevelMenu : MonoBehaviour
{
    [Header("UI Document")]
    public UIDocument uiDocument;

    private VisualElement root;
    private Button nextLevelButton;
    private Button replayLevelButton;
    private Button mainMenuButton;

    private void Start()
    {
        if (uiDocument == null)
        {
            Debug.LogError("WinLevelMenu: No UIDocument assigned!");
            return;
        }

        root = uiDocument.rootVisualElement;
        if (root == null)
        {
            Debug.LogError("WinLevelMenu: rootVisualElement is null!");
            return;
        }

        // Find buttons
        nextLevelButton = root.Q<Button>("NextLevelButton");
        replayLevelButton = root.Q<Button>("ReplayLevelButton");
        mainMenuButton = root.Q<Button>("MainMenuButton");

        // Register callbacks
        if (nextLevelButton != null)
            nextLevelButton.RegisterCallback<ClickEvent>(OnNextLevel);
        if (replayLevelButton != null)
            replayLevelButton.RegisterCallback<ClickEvent>(OnReplayLevel);
        if (mainMenuButton != null)
            mainMenuButton.RegisterCallback<ClickEvent>(OnMainMenu);
    }

    private void OnNextLevel(ClickEvent evt)
    {
        int current = SceneManager.GetActiveScene().buildIndex;
        int next = current + 2;

        if (next < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(next);
        else
        {
            Debug.LogWarning("No next scene found in Build Settings.");
            SceneManager.LoadScene(0);
        }
        
    }

    private void OnReplayLevel(ClickEvent evt)
    {
        int current = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(current - 1);
    }

    private void OnMainMenu(ClickEvent evt)
    {
        SceneManager.LoadScene(0);
    }
}
