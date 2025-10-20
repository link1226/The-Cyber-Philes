using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class UIScreenManager : MonoBehaviour
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

    void Awake()
    {
        doc = GetComponent<UIDocument>();
        lookup = new Dictionary<string, VisualTreeAsset>();

        foreach (var s in screens)
            lookup[s.name] = s.asset;

        ShowScreen("1010");
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
    }

    private void WireButtons(string screenName)
    {

        if (!screenName.Equals("1010") && !screenName.Equals("1011"))
        {
            // Show dock on all screens not the login screen
            root.Q<Button>("DockFileFindButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("1110"));
            root.Q<Button>("DockWebBrowserButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("1130"));
            root.Q<Button>("DockPasswordManButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("1150"));
            root.Q<Button>("DockTextViewButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("1122"));
        }

        // Add buttons for each screen
        switch (screenName)
        {
            case "1010": // PC Login
            case "1011":
                var ok = root.Q<Button>("OKButton");
                var pw = root.Q<TextField>("PasswordField");
                ok.clicked += () =>
                {
                    if (pw.value == "123")
                        ShowScreen("1100"); // Desktop
                    else
                        ShowScreen("1011"); // Incorrect login
                };
                break;

            case "1110":
                root.Q<Button>("CloseButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("1100"));
                root.Q<Button>("2faButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("1121"));
                root.Q<Button>("passcodeButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("1122"));
                break;

            case "1121":
            case "1122":
                root.Q<Button>("CloseButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("1110"));
                root.Q<Button>("CloseButtonBoth")?.RegisterCallback<ClickEvent>(_ => ShowScreen("1100"));
                break;
        }
    }
}
