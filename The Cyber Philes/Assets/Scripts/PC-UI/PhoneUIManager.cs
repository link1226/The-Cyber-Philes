using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class UISPhonenManager : MonoBehaviour
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

    void Awake()
    {
        doc = GetComponent<UIDocument>();
        lookup = new Dictionary<string, VisualTreeAsset>();

        foreach (var s in screens)
            lookup[s.name] = s.asset;

        ShowScreen("1200");
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

        if (!screenName.Equals("1010") && !screenName.Equals("1011"))
        {
            // Show dock on all screens not the login screen

        }

        // Add buttons for each screen
        switch (screenName)
        {
            case "1200": // Lock screen
                root.Q<Button>("ScreenTap")?.RegisterCallback<ClickEvent>(_ => ShowScreen("1201"));
                break;

            case "1201": // Passcode screen
                root.Q<Label>("Code").text = "";
                root.Q<Button>("Back")?.RegisterCallback<ClickEvent>(_ => ShowScreen("1200"));

                var codeLabel = root.Q<Label>("Code");

                string[] buttonNames =
                {
                    "Zero", "One", "Two", "Three", "Four",
                    "Five", "Six", "Seven", "Eight", "Nine"
                };

                for (int i = 0; i < buttonNames.Length; i++)
                {
                    var button = root.Q<Button>(buttonNames[i]);
                    if (button == null) continue;

                    int num = i; // capture loop variable
                    button.RegisterCallback<ClickEvent>(_ =>
                    {
                        if (codeLabel.text.Length < 6)
                            codeLabel.text += num.ToString();
                    });
                }

                var ok = root.Q<Button>("OK");
                ok.clicked += () =>
                {
                    if (codeLabel.text == LevelManager.Instance.phonePasscode)
                        ShowScreen("1210"); // Home screen
                    else
                        ShowScreen("1201"); // Incorrect login
                };
                break;


            case "1210": // Home Screen
                root.Q<Button>("Auth")?.RegisterCallback<ClickEvent>(_ => ShowScreen("1220"));
                break;
        }
    }

    void Update()
    {
        if (currentScreen == "1220")
        {
            string code = LevelManager.Instance.twoFACode;

            // Must be 6 digits
            if (!string.IsNullOrEmpty(code) && code.Length == 6)
            {
                code = code.Insert(3, " "); // Puts in middle space
            }

            root.Q<Label>("Code").text = code;
        }
    }

}
