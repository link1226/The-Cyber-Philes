using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Unity.Tutorials.Core.Editor;
using System.Runtime.CompilerServices;



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

        // Re-register text fields with keyboard
        if (keyboardConnector != null)
            keyboardConnector.RegisterTextFields(root);
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
                    if (pw.value == LevelManager.Instance.pcPassword)
                        ShowScreen("1100"); // Desktop
                    else
                        ShowScreen("1011"); // Incorrect login
                };
                break;

            case "1110": // FileFind
                root.Q<Button>("CloseButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("1100"));
                root.Q<Button>("2faButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("1121"));
                root.Q<Button>("passcodeButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("1122"));
                break;

            case "1121":
            case "1122": // TextView
                root.Q<Button>("CloseButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("1110"));
                root.Q<Button>("CloseButtonBoth")?.RegisterCallback<ClickEvent>(_ => ShowScreen("1100"));
                break;
            case "1125": // TextView DoorCode
                string doorCode = LevelManager.Instance.doorPasscode;
                doorCode = doorCode.Insert(6, " "); // Puts in middle space
                doorCode = doorCode.Insert(3, " "); // Puts in middle space

                root.Q<Label>("DoorCode").text = doorCode;
                root.Q<Button>("CloseButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("1140"));
                root.Q<Button>("CloseButtonBoth")?.RegisterCallback<ClickEvent>(_ => ShowScreen("1100"));
                break;

            case "1130":
            case "1131": // Web login
                root.Q<Button>("CloseButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("1100"));
                root.Q<Button>("ResetButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("1132"));

                var login = root.Q<Button>("LoginButton");
                var pwf = root.Q<TextField>("PasswordField");
                login.clicked += () =>
                {
                    if (pwf.value.Equals(LevelManager.Instance.webPassword) && !pwf.value.Equals(""))
                        ShowScreen("1139"); // 2FA
                    else
                        ShowScreen("1131"); // Incorrect login
                };
                break;

            case "1132": // Web Security Q
                root.Q<Button>("CloseButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("1100"));
                var verify = root.Q<Button>("VerifyButton");
                var secq = root.Q<TextField>("SecurityField");
                verify.clicked += () =>
                {
                    if (secq.value == LevelManager.Instance.securityQuestion)
                        ShowScreen("1134"); // Reset password
                    else
                        ShowScreen("1133"); // No auth
                };
                break;

            case "1133": // Web No Authentication
                root.Q<Button>("CloseButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("1100"));
                root.Q<Button>("ReturnButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("1130"));
                break;

            case "1134": // Web Set Password
                root.Q<Button>("CloseButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("1100"));

                var set = root.Q<Button>("SetButton");
                var np = root.Q<TextField>("NewPasswordField");
                var npv = root.Q<TextField>("NewPasswordVerifyField");

                // This is the container that holds the background.
                var bg = root.Q<VisualElement>("BackgroundPanel");

                string lastValue = "";

                void UpdatePasswordStrengthUI()
                {
                    int strength = passwordStrengthCheck.CheckPasswordStrength(np.value);
                    Debug.Log("Strength is " + strength);

                    switch (strength)
                    {
                        case 1:
                            // Weak → background 1135
                            bg.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("1135"));
                            break;

                        case 2:
                            // Medium → background 1136
                            bg.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("1136"));
                            break;

                        case 3:
                            // Strong → background 1137
                            bg.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("1137"));
                            break;
                    }
                }

                // --- PHYSICAL KEYBOARD ---
                np.RegisterValueChangedCallback(evt => UpdatePasswordStrengthUI());

                // --- VR SPATIAL KEYBOARD ---
                np.RegisterCallback<InputEvent>(_ => UpdatePasswordStrengthUI());

                // --- FALLBACK POLL (covers any missed updates)
                root.schedule.Execute(() =>
                {
                    if (np.value != lastValue)
                    {
                        lastValue = np.value;
                        UpdatePasswordStrengthUI();
                    }
                }).Every(10); // checks every 10ms, adjust if needed

                // Initial background
                UpdatePasswordStrengthUI();

                set.clicked += () =>
                {
                    int strength = passwordStrengthCheck.CheckPasswordStrength(np.value);

                    // Passwords must match FIRST
                    if (!np.value.Equals(npv.value))
                    {
                        ShowScreen("1134");
                        return;
                    }

                    // Now strength decides what happens
                    switch (strength)
                    {
                        case 1: // Weak
                        case 2: // Medium
                            ShowScreen("1134");
                            break;

                        case 3: // Strong
                                // Accept new password
                            LevelManager.Instance.webPassword = np.value;
                            ShowScreen("1139");
                            break;
                    }
                };

                break;
                


            case "1139": // Web 2FA
                root.Q<Button>("CloseButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("1100"));
                var verify2 = root.Q<Button>("VerifyButton");
                var code = root.Q<TextField>("CodeField");
                verify2.clicked += () =>
                {
                    if (code.value.Equals(LevelManager.Instance.twoFACode)) 
                        ShowScreen("1140"); // Successful Login
                    else
                        ShowScreen("1133"); // No auth
                };
                break;

            case "1140": // Company Drive Website
                root.Q<Button>("CloseButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("1100"));
                root.Q<Button>("DoorCodeButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("1125"));
                break;

            case "1150": // Password Manager
                root.Q<Button>("CloseButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("1100"));
                root.Q<Button>("LocalButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("1151"));
                root.Q<Button>("PasscodeButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("1152"));
                root.Q<Button>("CompDriveButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("1153"));
                break;

            case "1151": // Password Entries
            case "1152":
            case "1153":
                string pwval;

                switch (screenName)
                {
                    case "1151":
                        pwval = LevelManager.Instance.pcPassword;
                        break;
                    case "1152":
                        pwval = LevelManager.Instance.phonePasscode;
                        break;
                    case "1153":
                        pwval = LevelManager.Instance.webPassword;
                        break;
                    default:
                        pwval = "";
                        break;
                }

                root.Q<Label>("PasswordLabel").text = pwval;
                root.Q<Button>("CloseButtonBoth")?.RegisterCallback<ClickEvent>(_ => ShowScreen("1100"));
                root.Q<Button>("CloseButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("1150"));
                root.Q<Button>("OKButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("1150"));
                break;
        }
    }
}
