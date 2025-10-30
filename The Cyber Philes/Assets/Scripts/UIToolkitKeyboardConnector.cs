//using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit.Samples.SpatialKeyboard;

public class UIToolkitKeyboardConnector : MonoBehaviour
{
    [Tooltip("Assign the UIDocuments you want to watch for TextFields.")]
    public UIDocument[] uiDocuments;

    [Tooltip("Enable debug logs for troubleshooting")]
    public bool debugLogs = false;

    TextField _activeField;
    bool _listenersRegistered = false;

    void Awake()
    {
        // Register keyboard listeners once (if keyboard exists at Awake)
        TryRegisterKeyboardListeners();

        // Register fields for any UIDocuments assigned in inspector
        foreach (var doc in uiDocuments)
        {
            if (doc == null) continue;
            RegisterTextFields(doc.rootVisualElement);
        }
    }

    void TryRegisterKeyboardListeners()
    {
        if (_listenersRegistered) return;

        if (GlobalNonNativeKeyboard.instance?.keyboard == null)
        {
            if (debugLogs) Debug.Log("[UITKConnector] keyboard instance not ready yet.");
            // Try again next frame (keyboard prefab may be instantiated in Awake of manager)
            this.Invoke(nameof(TryRegisterKeyboardListeners), 0.05f);
            return;
        }

        var k = GlobalNonNativeKeyboard.instance.keyboard;
        k.onTextUpdated.AddListener(OnKeyboardTextUpdated);
        k.onClosed.AddListener(OnKeyboardClosed);

        _listenersRegistered = true;
        if (debugLogs) Debug.Log("[UITKConnector] Registered keyboard listeners.");
    }

    // Make this public so UIScreenManager can call it when swapping UXML
    public void RegisterTextFields(VisualElement root)
    {
        if (root == null) return;

        var fields = root.Query<TextField>().ToList();
        if (debugLogs) Debug.Log($"[UITKConnector] Registering {fields.Count} textfields from root '{root.name ?? "root"}'.");

        foreach (var field in fields)
        {
            // Avoid duplicate registration: remove previous callbacks with the same signature if possible
            // (UI Toolkit doesn't provide an easy remove for anonymous delegates, so guard by sending only once)
            // We will still register; duplicates are unlikely if you only call this when creating the tree.

            field.RegisterCallback<FocusInEvent>(evt =>
            {
                _activeField = field;
                if (debugLogs) Debug.Log($"[UITKConnector] FocusIn -> {_activeField.name}");

                // Show keyboard with the current text (do not remove keyboard listeners)
                GlobalNonNativeKeyboard.instance?.ShowKeyboard(field.value);
            });

            field.RegisterCallback<FocusOutEvent>(evt =>
            {
                // Ignore blur events that happen right after a keyboard key press.
                if (GlobalNonNativeKeyboard.instance != null &&
                    GlobalNonNativeKeyboard.instance.isActiveAndEnabled)
                {
                    if (debugLogs) Debug.Log($"[UITKConnector] Ignored FocusOut from active keyboard for {field.name}");
                    return; // don’t clear _activeField
                }

                if (_activeField == field)
                {
                    if (debugLogs) Debug.Log($"[UITKConnector] FocusOut -> clearing activeField (was {field.name})");
                    _activeField = null;
                }
            });

        }
    }

    // Keyboard events use KeyboardTextEventArgs in XRI 3.2.1
    void OnKeyboardTextUpdated(KeyboardTextEventArgs args)
    {
        // Defensive checks
        if (!_listenersRegistered)
        {
            if (debugLogs) Debug.Log("[UITKConnector] Received text update but listeners not fully registered.");
            return;
        }

        if (_activeField == null)
        {
            // No active field to write to — still useful to log
            if (debugLogs) Debug.Log("[UITKConnector] Text update received but no active field.");
            return;
        }

        var keyboard = args.keyboard ?? GlobalNonNativeKeyboard.instance?.keyboard;
        if (keyboard == null)
        {
            if (debugLogs) Debug.LogWarning("[UITKConnector] Keyboard reference missing on text update.");
            return;
        }

        var newText = keyboard.text;

        if (debugLogs) Debug.Log($"[UITKConnector] OnKeyboardTextUpdated -> newText length {newText?.Length ?? 0}");

        // Update the TextField directly with SetValueWithoutNotify to avoid any binding loops / callbacks.
        _activeField.SetValueWithoutNotify(newText);

        // Defer focus reapplication to next frame so XR pointer blurs complete first.
        // schedule.Execute with 0 delay runs at next opportunity after event processing.
        _activeField.schedule.Execute(() =>
        {
            if (_activeField == null) return;
            try
            {
                _activeField.Focus();

                // Move caret to end if those properties exist (they normally do)
                var len = _activeField.value?.Length ?? 0;
                _activeField.cursorIndex = len;
                _activeField.selectIndex = len;
            }
            catch (System.Exception e)
            {
                // Some UI Toolkit versions may not expose cursorIndex/selectIndex; ignore if not present.
                if (debugLogs) Debug.LogWarning("[UITKConnector] Focus or caret update failed: " + e.Message);
            }
        }).StartingIn(0);
    }

    void OnKeyboardClosed(KeyboardTextEventArgs args)
    {
        if (debugLogs) Debug.Log("[UITKConnector] Keyboard closed. Clearing active field and blurring.");
        _activeField?.Blur();
        _activeField = null;
    }
}
