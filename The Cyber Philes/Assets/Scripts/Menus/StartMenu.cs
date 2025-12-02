//using UnityEngine;
//using UnityEngine.SceneManagement;
//using UnityEngine.UIElements;

//public class StartMenu : MonoBehaviour
//{
//    [Header("Menu Documents")]
//    [SerializeField] private UIDocument startMenu;
//    [SerializeField] private UIDocument optionsMenu;
//    [SerializeField] private UIDocument aboutMenu;

//    private const string VolumePref = "UserVolume";

//    private Slider volumeSlider;
//    private UIDocument currentMenu;

//    private void Start()
//    {
//        // Start with the main menu visible
//        ShowMenu(startMenu);

//        // --- START MENU ---
//        var startRoot = startMenu.rootVisualElement;
//        startRoot.Q<Button>("StartButton")?.RegisterCallback<ClickEvent>(OnStartClicked);
//        startRoot.Q<Button>("OptionsButton")?.RegisterCallback<ClickEvent>(evt => ShowMenu(optionsMenu));
//        startRoot.Q<Button>("AboutButton")?.RegisterCallback<ClickEvent>(evt => ShowMenu(aboutMenu));
//        startRoot.Q<Button>("QuitButton")?.RegisterCallback<ClickEvent>(OnQuitClicked);

//        // --- OPTIONS MENU ---
//        var optionsRoot = optionsMenu.rootVisualElement;
//        optionsRoot.Q<Button>("OptionsBackButton")?.RegisterCallback<ClickEvent>(evt => ShowMenu(startMenu));

//        volumeSlider = optionsRoot.Q<Slider>("OptionsVolumeSlider");
//        LoadUserSettings();

//        if (volumeSlider != null)
//            volumeSlider.RegisterValueChangedCallback(evt => OnVolumeChanged(evt.newValue));

//        // --- ABOUT MENU ---
//        var aboutRoot = aboutMenu.rootVisualElement;
//        aboutRoot.Q<Button>("AboutBackButton")?.RegisterCallback<ClickEvent>(evt => ShowMenu(startMenu));
//    }

//     //--- Menu Switching ---
//    private void ShowMenu(UIDocument menuToShow)
//    {
//        startMenu.rootVisualElement.style.display = DisplayStyle.None;
//        optionsMenu.rootVisualElement.style.display = DisplayStyle.None;
//        aboutMenu.rootVisualElement.style.display = DisplayStyle.None;

//        //startMenu.rootVisualElement.visible = false;
//        //optionsMenu.rootVisualElement.visible = false;
//        //aboutMenu.rootVisualElement.visible = false;

//        if (menuToShow != null)
//        {
//            menuToShow.rootVisualElement.style.display = DisplayStyle.Flex;
//            currentMenu = menuToShow;
//        }
//    }
//    //private void ShowMenu(UIDocument menuToShow)
//    //{
//    //    // Disable all menus completely
//    //    startMenu.gameObject.SetActive(false);
//    //    optionsMenu.gameObject.SetActive(false);
//    //    aboutMenu.gameObject.SetActive(false);

//    //    // Enable the chosen menu
//    //    if (menuToShow != null)
//    //    {
//    //        menuToShow.gameObject.SetActive(true);
//    //        currentMenu = menuToShow;
//    //    }
//    //}


//    // --- Button Handlers ---
//    private void OnStartClicked(ClickEvent evt)
//    {
//        int currentIndex = SceneManager.GetActiveScene().buildIndex;
//        int nextIndex = currentIndex + 1;

//        if (nextIndex < SceneManager.sceneCountInBuildSettings)
//            SceneManager.LoadScene(nextIndex);
//        else
//            Debug.LogWarning("No next scene found in Build Settings.");
//    }

//    private void OnQuitClicked(ClickEvent evt)
//    {
//        Debug.Log("Quitting game...");
//        Application.Quit();

//#if UNITY_EDITOR
//        UnityEditor.EditorApplication.isPlaying = false;
//#endif
//    }

//    // --- Settings Persistence ---
//    private void LoadUserSettings()
//    {
//        float volume = PlayerPrefs.GetFloat(VolumePref, 0.5f);

//        if (volumeSlider != null)
//            volumeSlider.value = volume;

//        ApplyVolume(volume);
//    }

//    private void OnVolumeChanged(float newValue)
//    {
//        PlayerPrefs.SetFloat(VolumePref, newValue);
//        PlayerPrefs.Save();
//        ApplyVolume(newValue);
//    }

//    private void ApplyVolume(float volume)
//    {
//        AudioListener.volume = volume; // simple global volume control
//    }
//}


// Version 2 

//using UnityEngine;
//using UnityEngine.SceneManagement;
//using UnityEngine.UIElements;

//public class StartMenu : MonoBehaviour
//{
//    private UIDocument uiDocument;

//    private VisualElement startMenu;
//    private VisualElement optionsMenu;
//    private VisualElement aboutMenu;

//    private Slider volumeSlider;

//    private const string VolumePref = "UserVolume";

//    private void Awake()
//    {
//        uiDocument = GetComponent<UIDocument>();
//    }

//    private void Start()
//    {
//        var root = uiDocument.rootVisualElement;

//        // Grab each menu layer by its name in the UXML hierarchy
//        startMenu = root.Q<VisualElement>("StartMenu");
//        optionsMenu = root.Q<VisualElement>("OptionsMenu");
//        aboutMenu = root.Q<VisualElement>("AboutMenu");

//        // --- START MENU BUTTONS ---
//        BindButton(startMenu, "StartButton", OnStartClicked);
//        BindButton(startMenu, "OptionsButton", evt => ShowMenu(optionsMenu));
//        BindButton(startMenu, "AboutButton", evt => ShowMenu(aboutMenu));
//        BindButton(startMenu, "QuitButton", OnQuitClicked);

//        // --- OPTIONS MENU ---
//        BindButton(optionsMenu, "OptionsBackButton", evt => ShowMenu(startMenu));
//        volumeSlider = optionsMenu.Q<Slider>("VolumeSlider");
//        if (volumeSlider != null)
//        {
//            LoadUserSettings();
//            volumeSlider.RegisterValueChangedCallback(evt => OnVolumeChanged(evt.newValue));
//        }

//        // --- ABOUT MENU ---
//        BindButton(aboutMenu, "AboutBackButton", evt => ShowMenu(startMenu));

//        // Start with Start Menu visible, others hidden
//        ShowMenu(startMenu);
//    }

//    private void BindButton(VisualElement parent, string name, EventCallback<ClickEvent> callback)
//    {
//        var button = parent.Q<Button>(name);
//        if (button != null)
//            button.RegisterCallback(callback);
//        else
//            Debug.LogWarning($"Button '{name}' not found in {parent.name}");
//    }

//    private void ShowMenu(VisualElement menuToShow)
//    {
//        // Hide all
//        startMenu.style.display = DisplayStyle.None;
//        optionsMenu.style.display = DisplayStyle.None;
//        aboutMenu.style.display = DisplayStyle.None;

//        // Show the one we want
//        if (menuToShow != null)
//            menuToShow.style.display = DisplayStyle.Flex;
//    }

//    private void OnStartClicked(ClickEvent evt)
//    {
//        int currentIndex = SceneManager.GetActiveScene().buildIndex;
//        int nextIndex = currentIndex + 1;

//        if (nextIndex < SceneManager.sceneCountInBuildSettings)
//            SceneManager.LoadScene(nextIndex);
//        else
//            Debug.LogWarning("No next scene found in Build Settings.");
//    }

//    private void OnQuitClicked(ClickEvent evt)
//    {
//        Debug.Log("Quitting game...");
//        Application.Quit();
//#if UNITY_EDITOR
//        UnityEditor.EditorApplication.isPlaying = false;
//#endif
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
//}

// Version 3

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [Header("UI Document")]
    public UIDocument uiDocument;

    [Header("Menu Panels")]
    public VisualElement startMenu;
    public VisualElement optionsMenu;
    public VisualElement aboutMenu;
    public VisualElement startSubmenu;

    private Slider volumeSlider;
    private const string VolumePref = "UserVolume";

    // bool to control Level 2 visiblity in menu
    //public bool unlockedLevel2 = false;

    private void Start()
    {
        if (uiDocument == null)
        {
            Debug.LogError("MenuManager: No UIDocument assigned!");
            return;
        }

        var root = uiDocument.rootVisualElement;
        if (root == null)
        {
            Debug.LogError("MenuManager: UIDocument has no rootVisualElement yet!");
            return;
        }

        // --- If not assigned manually, try auto-find them once ---
        if (startMenu == null) startMenu = root.Q<VisualElement>("StartMenu");
        if (optionsMenu == null) optionsMenu = root.Q<VisualElement>("OptionsMenu");
        if (aboutMenu == null) aboutMenu = root.Q<VisualElement>("AboutMenu");
        if (startSubmenu == null) startSubmenu = root.Q<VisualElement>("StartSubmenu");

        // --- Bind Start Menu buttons ---
        BindButton(startMenu, "StartButton", evt => ShowMenu(startSubmenu));
        BindButton(startMenu, "OptionsButton", evt => ShowMenu(optionsMenu));
        BindButton(startMenu, "AboutButton", evt => ShowMenu(aboutMenu));
        BindButton(startMenu, "QuitButton", OnQuitClicked);

        // --- Start Submenu ---
        BindButton(startSubmenu, "Level1", evt => SceneManager.LoadScene(1));
        BindButton(startSubmenu, "Level2", evt => SceneManager.LoadScene(4));
        BindButton(startSubmenu, "Back", evt => ShowMenu(startMenu));

        // --- Options Menu ---
        BindButton(optionsMenu, "OptionsBackButton", evt => ShowMenu(startMenu));
        volumeSlider = optionsMenu?.Q<Slider>("VolumeSlider");
        if (volumeSlider != null)
        {
            LoadUserSettings();
            volumeSlider.RegisterValueChangedCallback(evt => OnVolumeChanged(evt.newValue));
        }

        // --- About Menu ---
        BindButton(aboutMenu, "AboutBackButton", evt => ShowMenu(startMenu));

        // Start with Start Menu visible
        ShowMenu(startMenu);
    }

    private void BindButton(VisualElement parent, string name, EventCallback<ClickEvent> callback)
    {
        if (parent == null)
        {
            Debug.LogWarning($"BindButton: parent is null for '{name}'");
            return;
        }
        var button = parent.Q<Button>(name);
        if (button != null)
            button.RegisterCallback(callback);
        else
            Debug.LogWarning($"Button '{name}' not found in {parent.name}");
    }

    private void ShowMenu(VisualElement menuToShow)
    {
        if (startMenu != null) startMenu.style.display = DisplayStyle.None;
        if (optionsMenu != null) optionsMenu.style.display = DisplayStyle.None;
        if (aboutMenu != null) aboutMenu.style.display = DisplayStyle.None;
        if (startSubmenu != null) startSubmenu.style.display = DisplayStyle.None;

        if (menuToShow != null)
            menuToShow.style.display = DisplayStyle.Flex;

        if (menuToShow == startSubmenu)
        {
            bool unlockedLevel2 = PlayerPrefs.GetInt("UnlockedLevel2", 0) == 1;
            if (!unlockedLevel2) startSubmenu.Q<Button>("Level2").style.display = DisplayStyle.None;
            else startSubmenu.Q<Button>("Level2").style.display = DisplayStyle.Flex;
        }
    }

    private void OnQuitClicked(ClickEvent evt)
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
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
}


