using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Unity.Tutorials.Core.Editor;
using System.Runtime.CompilerServices;



public class Lvl2PCUIManager : MonoBehaviour
{
    [System.Serializable]
    public struct ScreenLink
    {
        public string name;
        public VisualTreeAsset asset;
    }

    [Header("Screens")]
    public List<ScreenLink> screens = new List<ScreenLink>();

    private UIDocument doc;
    private Dictionary<string, VisualTreeAsset> lookup;
    private VisualElement root;
    private string currentScreen;

    [Header("Keyboard Connector")]
    public UIToolkitKeyboardConnector keyboardConnector;

    //[Header("Password Stength Checker Script")]
    private PasswordStrengthCheck passwordStrengthCheck = new PasswordStrengthCheck();

    //private int PasswordStrengthCheck(string password)
    //{
    //    if (string.IsNullOrEmpty(password)) 
    //        return 0;
    //    if (password.Length < 6)
    //        return 1;
    //    if (password.Length < 8)
    //        return 2;
    //    return 3;
    //}

    void Awake()
    {
        doc = GetComponent<UIDocument>();
        lookup = new Dictionary<string, VisualTreeAsset>();

        foreach (var s in screens)
            lookup[s.name] = s.asset;

        ShowScreen("2100");
    }

    public void ShowScreen(string name)
    {
        if (!lookup.ContainsKey(name))
        {
            Debug.LogError($"❌ Screen {name} not found! Available keys: {string.Join(", ", lookup.Keys)}");
            return;
        }

        doc.visualTreeAsset = lookup[name];
        root = doc.rootVisualElement;
        currentScreen = name;

        // Re-bind buttons for this screen
        WireButtons(name);

        // Re-register text fields with keyboard
        if (keyboardConnector != null)
            keyboardConnector.RegisterTextFields(root);
    }

    private void WireButtons(string screenName)
    {

        if (int.Parse(screenName) >= 2100 && int.Parse(screenName) < 2127)
        {
            // Admin Dock
            root.Q<Button>("DockControlPaneButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2121"));
        }

        // Add buttons for each screen
        switch (screenName)
        {
            case "2121": // Control Pane
                root.Q<Button>("CloseButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2100"));
                root.Q<Button>("GeorgeButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2122"));
                root.Q<Button>("SoftwareButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2125"));
                break;

            case "2122": 
                root.Q<Button>("CloseAllButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2100"));
                root.Q<Button>("CloseButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2121"));
                root.Q<Button>("OKButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2121"));
                root.Q<Button>("ResetButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2123"));
                root.Q<Button>("SoftwareButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2125"));
                break;

            case "2123": 
                root.Q<Button>("CloseAllButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2100"));
                root.Q<Button>("CloseBothButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2121"));
                root.Q<Button>("CloseButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2122"));

                root.Q<Button>("OKButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2122"));
                root.Q<Button>("OKBackButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2121"));
                break;

            case "2125":
            case "2126":
                root.Q<Button>("CloseButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2100"));
                root.Q<Button>("UsersButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2121"));
                root.Q<Button>("InstallButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2126"));
                break;
        }
    }
}
