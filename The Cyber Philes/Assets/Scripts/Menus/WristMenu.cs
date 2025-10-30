//using UnityEngine;
//using UnityEngine.SceneManagement;
//using UnityEngine.UIElements;
//using UnityEngine.InputSystem;

//public class WristMenu : MonoBehaviour
//{
//    [Header("UI Document")]
//    public UIDocument uiDocument;
//    public InputActionReference toggleMenuAction;

//    private VisualElement root;
//    private VisualElement wristMenu;
//    private VisualElement optionsMenu;
//    private Button optionsButton;
//    private Button mainMenuButton;
//    private Button backButton;
//    private Slider volumeSlider;

//    private const string VolumePref = "UserVolume";

//    private void Start()
//    {
//        if (uiDocument == null)
//        {
//            Debug.LogError("WristMenu: No UIDocument assigned!");
//            return;
//        }

//        root = uiDocument.rootVisualElement;
//        if (root == null)
//        {
//            Debug.LogError("WristMenu: rootVisualElement is null!");
//            return;
//        }

//        // Main containers
//        wristMenu = root.Q<VisualElement>("WristMenu");
//        optionsMenu = root.Q<VisualElement>("OptionsMenu");

//        if (wristMenu == null || optionsMenu == null)
//        {
//            Debug.LogError("WristMenu: Missing one or more main containers (WristMenu / OptionsMenu).");
//            return;
//        }

//        // Wrist menu buttons
//        optionsButton = wristMenu.Q<Button>("OptionsButton");
//        mainMenuButton = wristMenu.Q<Button>("MainMenuButton");

//        // Options menu controls
//        volumeSlider = optionsMenu.Q<Slider>("OptionsVolumeSlider");
//        backButton = optionsMenu.Q<Button>("OptionsBackButton");

//        // Register button actions
//        if (optionsButton != null)
//            optionsButton.RegisterCallback<ClickEvent>(evt => ShowOptionsMenu());

//        if (mainMenuButton != null)
//            mainMenuButton.RegisterCallback<ClickEvent>(evt => SceneManager.LoadScene(0));

//        if (backButton != null)
//            backButton.RegisterCallback<ClickEvent>(evt => ShowWristMenu());

//        // Volume control setup
//        if (volumeSlider != null)
//        {
//            LoadUserSettings();
//            volumeSlider.RegisterValueChangedCallback(evt => OnVolumeChanged(evt.newValue));
//        }

//        ShowWristMenu(); // Start with wrist menu visible
//    }

//    private void ShowOptionsMenu()
//    {
//        wristMenu.style.display = DisplayStyle.None;
//        optionsMenu.style.display = DisplayStyle.Flex;
//    }

//    private void ShowWristMenu()
//    {
//        wristMenu.style.display = DisplayStyle.Flex;
//        optionsMenu.style.display = DisplayStyle.None;
//    }

//    private void LoadUserSettings()
//    {
//        float volume = PlayerPrefs.GetFloat(VolumePref, 0.5f);
//        if (volumeSlider != null)
//            volumeSlider.value = volume;
//        AudioListener.volume = volume;
//    }

//    private void OnVolumeChanged(float newValue)
//    {
//        PlayerPrefs.SetFloat(VolumePref, newValue);
//        PlayerPrefs.Save();
//        AudioListener.volume = newValue;
//    }

//    // Optional: toggle menu visibility entirely (if you bind to controller button)
//    public void ToggleMenu()
//    {
//        bool isVisible = wristMenu.style.display == DisplayStyle.Flex || optionsMenu.style.display == DisplayStyle.Flex;
//        wristMenu.style.display = isVisible ? DisplayStyle.None : DisplayStyle.Flex;
//        optionsMenu.style.display = DisplayStyle.None;
//    }
//}


// Version 2

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class WristMenu : MonoBehaviour
{
    [Header("UI Document")]
    public UIDocument uiDocument;

    [Header("Input Action")]
    public InputActionReference toggleMenuAction; // assign in Inspector

    private VisualElement root;
    private VisualElement wristMenu;
    private VisualElement optionsMenu;

    private Button optionsButton;
    private Button mainMenuButton;
    private Button backButton;
    private Slider volumeSlider;

    private const string VolumePref = "UserVolume";

    private void Start()
    {
        if (uiDocument == null)
        {
            Debug.LogError("WristMenu: No UIDocument assigned!");
            return;
        }

        root = uiDocument.rootVisualElement;
        if (root == null)
        {
            Debug.LogError("WristMenu: rootVisualElement is null!");
            return;
        }

        // Find the two main containers
        wristMenu = root.Q<VisualElement>("WristMenu");
        optionsMenu = root.Q<VisualElement>("OptionsMenu");

        if (wristMenu == null || optionsMenu == null)
        {
            Debug.LogError("WristMenu: WristMenu or OptionsMenu VisualElement not found.");
            return;
        }

        // Wrist menu buttons
        optionsButton = wristMenu.Q<Button>("OptionsButton");
        mainMenuButton = wristMenu.Q<Button>("MainMenuButton");

        // Options menu controls
        backButton = optionsMenu.Q<Button>("OptionsBackButton");
        volumeSlider = optionsMenu.Q<Slider>("OptionsVolumeSlider");

        // Register button callbacks
        if (optionsButton != null)
            optionsButton.RegisterCallback<ClickEvent>(evt => ShowOptionsMenu());

        if (mainMenuButton != null)
            mainMenuButton.RegisterCallback<ClickEvent>(evt => SceneManager.LoadScene(0));

        if (backButton != null)
            backButton.RegisterCallback<ClickEvent>(evt => ShowWristMenu());

        // Volume slider setup
        if (volumeSlider != null)
        {
            LoadUserSettings();
            volumeSlider.RegisterValueChangedCallback(evt => OnVolumeChanged(evt.newValue));
        }

        // --- Menu closed by default ---
        wristMenu.style.display = DisplayStyle.None;
        optionsMenu.style.display = DisplayStyle.None;
    }

    private void ShowOptionsMenu()
    {
        wristMenu.style.display = DisplayStyle.None;
        optionsMenu.style.display = DisplayStyle.Flex;
    }

    private void ShowWristMenu()
    {
        wristMenu.style.display = DisplayStyle.Flex;
        optionsMenu.style.display = DisplayStyle.None;
    }

    private void LoadUserSettings()
    {
        float volume = PlayerPrefs.GetFloat(VolumePref, 0.5f);
        if (volumeSlider != null)
            volumeSlider.value = volume;
        AudioListener.volume = volume;
    }

    private void OnVolumeChanged(float newValue)
    {
        PlayerPrefs.SetFloat(VolumePref, newValue);
        PlayerPrefs.Save();
        AudioListener.volume = newValue;
    }

    // Toggle menu visibility from input
    public void ToggleMenu()
    {
        bool isVisible = wristMenu.style.display == DisplayStyle.Flex || optionsMenu.style.display == DisplayStyle.Flex;
        wristMenu.style.display = isVisible ? DisplayStyle.None : DisplayStyle.Flex;
        optionsMenu.style.display = DisplayStyle.None;
    }

    // --- Input System listeners ---
    private void OnEnable()
    {
        if (toggleMenuAction != null)
        {
            toggleMenuAction.action.performed += OnToggleMenu;
            toggleMenuAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (toggleMenuAction != null)
        {
            toggleMenuAction.action.performed -= OnToggleMenu;
            toggleMenuAction.action.Disable();
        }
    }

    private void OnToggleMenu(InputAction.CallbackContext ctx)
    {
        ToggleMenu();
    }
}


