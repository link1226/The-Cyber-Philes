using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

namespace NavKeypad
{
    [RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable))]
    public class KeypadButton : MonoBehaviour
    {
        [Header("Value")]
        [SerializeField] private string value;

        [Header("Animation Settings")]
        [SerializeField] private float pressSpeed = 0.1f;
        [SerializeField] private float moveDist = 0.0025f;
        [SerializeField] private float pressedDuration = 0.1f;

        [Header("References")]
        [SerializeField] private Keypad keypad;

        private bool moving;
        private UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable interactable;

        private void Awake()
        {
            interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();
            interactable.selectEntered.AddListener(OnSelectEntered);
        }

        private void OnSelectEntered(SelectEnterEventArgs args)
        {
            if (!moving)
            {
                keypad.AddInput(value);
                StartCoroutine(MoveSmooth());
            }
        }

        private IEnumerator MoveSmooth()
        {
            moving = true;
            Vector3 startPos = transform.localPosition;
            Vector3 endPos = startPos + new Vector3(0, 0, -moveDist);

            float elapsed = 0;
            while (elapsed < pressSpeed)
            {
                elapsed += Time.deltaTime;
                transform.localPosition = Vector3.Lerp(startPos, endPos, elapsed / pressSpeed);
                yield return null;
            }

            yield return new WaitForSeconds(pressedDuration);

            elapsed = 0;
            while (elapsed < pressSpeed)
            {
                elapsed += Time.deltaTime;
                transform.localPosition = Vector3.Lerp(endPos, startPos, elapsed / pressSpeed);
                yield return null;
            }

            transform.localPosition = startPos;
            moving = false;
        }
    }
    }