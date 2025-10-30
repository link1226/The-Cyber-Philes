using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LoseLevelMenu : MonoBehaviour
{
    [Header("UI Document")]
    public UIDocument uiDocument;

    private VisualElement root;
    private Button replayLevelButton;
    private Button mainMenuButton;

    private void Start()
    {
        if (uiDocument == null)
        {
            Debug.LogError("LoseLevelMenu: No UIDocument assigned!");
            return;
        }

        root = uiDocument.rootVisualElement;
        if (root == null)
        {
            Debug.LogError("LoseLevelMenu: rootVisualElement is null!");
            return;
        }

        // Find buttons
        replayLevelButton = root.Q<Button>("ReplayLevelButton");
        mainMenuButton = root.Q<Button>("MainMenuButton");

        // Register callbacks
        if (replayLevelButton != null)
            replayLevelButton.RegisterCallback<ClickEvent>(OnReplayLevel);
        if (mainMenuButton != null)
            mainMenuButton.RegisterCallback<ClickEvent>(OnMainMenu);
    }

    private void OnReplayLevel(ClickEvent evt)
    {
        SceneManager.LoadScene(1);
    }

    private void OnMainMenu(ClickEvent evt)
    {
        SceneManager.LoadScene(0);
    }
}
