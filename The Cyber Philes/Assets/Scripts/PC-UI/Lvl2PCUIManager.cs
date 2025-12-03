using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Unity.Tutorials.Core.Editor;
using System.Runtime.CompilerServices;
using NavKeypad;
using UnityEngine.Events;
using System.Threading;



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
    [Header("Events")]
    [SerializeField] private UnityEvent onAccessGranted;
    public UnityEvent OnAccessGranted => onAccessGranted;

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

        if (name == "ComputerScreenAdmin")
            ShowScreen("2100");
        else
            ShowScreen("2200");
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

        if (int.Parse(screenName) >= 2100 && int.Parse(screenName) < 2200 && screenName!="2127")
        {
            // Admin Dock
            root.Q<Button>("DockControlPaneButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2121"));
        }

        if (int.Parse(screenName) >= 2200 && int.Parse(screenName) < 2300)
        {
            // George Dock
            root.Q<Button>("DockWebBrowserButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2230"));
            root.Q<Button>("DockEmailButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen(LevelManager2.Instance.inboxStage));
        }

        // Add buttons for each screen
        switch (screenName)
        {
            case "2121": // Control Pane
                root.Q<Button>("CloseButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2100"));
                root.Q<Button>("GeorgeButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2122"));
                root.Q<Button>("MarieButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2128"));
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
                LevelManager2.Instance.GeneratePCPassword();

                root.Q<Button>("CloseAllButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2100"));
                root.Q<Button>("CloseBothButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2121"));
                root.Q<Button>("CloseButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2122"));

                root.Q<Button>("OKButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2122"));
                root.Q<Button>("OKBackButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2121"));
                break;

            case "2125":
                root.Q<Button>("CloseButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2100"));
                root.Q<Button>("UsersButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2121"));
                root.Q<Button>("InstallButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2126"));
                break;
            case "2126":
                root.Q<Button>("CloseButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2100"));
                root.Q<Button>("UsersButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2121"));
                LevelManager2.Instance.softwareUpdated = true;
                break;

            case "2128":
                root.Q<Button>("CloseAllButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2100"));
                root.Q<Button>("CloseButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2121"));
                root.Q<Button>("OKButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2121"));
                root.Q<Button>("ResetButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2129"));
                root.Q<Button>("SoftwareButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2125"));
                break;

            case "2129": 
                root.Q<Button>("CloseAllButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2100"));
                root.Q<Button>("CloseBothButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2121"));
                root.Q<Button>("CloseButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2128"));

                root.Q<Button>("OKButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2128"));
                root.Q<Button>("OKBackButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2121"));
                LevelManager2.Instance.mariePasswordReset = true;
                break;


            case "2230": // Login
            case "2231":
                // Send the spam email upon opening the browser
                if (LevelManager2.Instance.inboxStage == "2250")
                    LevelManager2.Instance.inboxStage = "2251";

                root.Q<Button>("CloseButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2200"));
                root.Q<Button>("ForgotButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2232"));

                var login = root.Q<Button>("LoginButton");
                var pwf = root.Q<TextField>("PasswordField");
                login.clicked += () =>
                {
                    if (pwf.value.Equals(LevelManager2.Instance.webPassword) && !pwf.value.Equals(""))
                        ShowScreen("2239"); // 2FA
                    else
                        ShowScreen("2231"); // Incorrect login
                };
                break;

            case "2232": // Forgot
                root.Q<Button>("CloseButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2200"));
                root.Q<Button>("ReturnButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2230"));

                // Send valid reset email
                if (LevelManager2.Instance.inboxStage == "2251")
                    LevelManager2.Instance.inboxStage = "2253";
                break;

            case "2233": // New password
            case "2237": // TODO: Missing strength algorithm
                root.Q<Button>("CloseButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2200"));

                var set = root.Q<Button>("SetButton");
                var newp1 = root.Q<TextField>("PasswordField");
                var newp2 = root.Q<TextField>("PasswordFieldAgain");
                set.clicked += () =>
                {
                    if (newp1.value.Equals(newp2.value) && !newp1.value.Equals("")) 
                    {
                        LevelManager2.Instance.webPassword = newp1.value;
                        ShowScreen("2239"); // 2FA
                    }
                    else
                        ShowScreen("2237"); // Mismatch
                };
                break;

            case "2238": // No auth
                root.Q<Button>("CloseButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2200"));
                root.Q<Button>("ReturnButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2230"));
                break;

            case "2239":
                // Send the 2fa email
                if (LevelManager2.Instance.inboxStage != "2255")
                    LevelManager2.Instance.inboxStage = "2255";

                root.Q<Button>("CloseButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2200"));

                var verify = root.Q<Button>("VerifyButton");
                var twoFA = root.Q<TextField>("TwoFAField");
                verify.clicked += () =>
                {
                    if (twoFA.value.Equals(LevelManager2.Instance.twoFACode))
                        ShowScreen("2240"); // logged in
                    else
                        ShowScreen("2238"); // Incorrect auth
                };
                break;

            case "2240":
                root.Q<Button>("CloseButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2200"));
                var tab = root.Q<Button>("SecurityButton");
                tab.clicked += () =>
                {
                    if (LevelManager2.Instance.softwareUpdated 
                    && LevelManager2.Instance.deviceBlacklisted 
                    && LevelManager2.Instance.mariePasswordReset)
                        ShowScreen("2245"); // Tasks completed
                    else
                        ShowScreen("2244"); // No auth
                };
                root.Q<Button>("SoftwareButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2241"));
                root.Q<Button>("NetworkButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2242"));
                root.Q<Button>("PasswordButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2243"));

                root.Q<Label>("SoftwareDone").visible = LevelManager2.Instance.softwareUpdated;
                root.Q<Label>("NetworkDone").visible = LevelManager2.Instance.deviceBlacklisted;
                root.Q<Label>("PasswordDone").visible = LevelManager2.Instance.mariePasswordReset;

                break;

            case "2241":
            case "2243":
            case "2244":
                root.Q<Button>("CloseButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2200"));
                root.Q<Button>("BackButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2240"));
                break;

            case "2242":
                root.Q<Button>("CloseButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2200"));
                root.Q<Button>("BackButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2240"));

                var blacklist = root.Q<Button>("BlacklistButton");
                blacklist.clicked += () =>
                {
                    LevelManager2.Instance.deviceBlacklisted = true;
                    ShowScreen("2240");
                };
                break;

            case "2245":
                root.Q<Button>("CloseButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2200"));
                root.Q<Button>("BackButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2240"));
                root.Q<Button>("DoorButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2246"));
                break;

            case "2246":
                root.Q<Button>("CloseButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2200"));
                root.Q<Button>("BackButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2240"));

                var unlock = root.Q<Button>("UnlockButton");
                unlock.clicked += () =>
                {
                    onAccessGranted?.Invoke();
                    CountdownTimer.Instance.StopTimer();
                    ShowScreen("2245");
                };
                break;



            case "2250": // Empty Inbox
                root.Q<Button>("CloseButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2200"));
                break;

            case "2251": // Inbox just spam
                root.Q<Button>("CloseButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2200"));
                root.Q<Button>("ScamButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2252"));
                break;

            case "2252": // Spam Email
                root.Q<Button>("CloseButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2200"));
                root.Q<Button>("BackButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen(LevelManager2.Instance.inboxStage));
                root.Q<Button>("ScamAttackButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2257")); // Launch Scam
                break;

            case "2253": // Inbox Reset & Spam
                root.Q<Button>("CloseButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2200"));
                root.Q<Button>("ScamButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2252"));
                root.Q<Button>("ResetButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2254"));
                break;

            case "2254": // Reset Email
                root.Q<Button>("CloseButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2200"));
                root.Q<Button>("BackButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen(LevelManager2.Instance.inboxStage));
                root.Q<Button>("ResetButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2233")); // Open Web Browser
                break;

            case "2255": // Inbox 2FA, Reset, & Spam
                root.Q<Button>("CloseButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2200"));
                root.Q<Button>("ScamButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2252"));
                root.Q<Button>("ResetButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2254"));
                root.Q<Button>("CodeButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2256"));
                break;

            case "2256": // Code Email
                root.Q<Button>("CloseButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2200"));
                root.Q<Button>("BackButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen(LevelManager2.Instance.inboxStage));
                break;

            case "2257": // GOT HACKED
                root.Q<Button>("CloseButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen("2200"));
                root.Q<Button>("BackButton")?.RegisterCallback<ClickEvent>(_ => ShowScreen(LevelManager2.Instance.inboxStage));

                CountdownTimer.Instance.SetTimeLeft(15f);
                break;
        }
    }
}
