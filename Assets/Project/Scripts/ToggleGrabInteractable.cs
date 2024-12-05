using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ToggleGrabInteractable : MonoBehaviour
{
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor currentInteractor = null; // Changed to IXRSelectInteractor
    private bool isGrabbing = false;

    void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        // Manually disable the select and deselect to control it
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    void OnDestroy()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }

    void Update()
    {
        if (currentInteractor != null && !isGrabbing)
        {
            if (IsPrimaryButtonPressed())
            {
                ToggleGrab();
            }
        }
        else if (isGrabbing)
        {
            if (IsPrimaryButtonPressed())
            {
                ToggleGrab();
            }
        }
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        // When the gun is first grabbed, keep a reference to the interactor
        currentInteractor = args.interactorObject;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        // Clear the interactor when the object is released
        currentInteractor = null;
    }

    private bool IsPrimaryButtonPressed()
    {
        // Check if the primary button is pressed (modify as needed for XR input)
        return Input.GetButtonDown("Fire1"); // Replace with XR-specific input if needed
    }

    private void ToggleGrab()
    {
        if (!isGrabbing && grabInteractable != null && currentInteractor != null)
        {
            grabInteractable.interactionManager.SelectEnter(currentInteractor, grabInteractable);
            isGrabbing = true;
        }
        else if (isGrabbing && grabInteractable != null && currentInteractor != null)
        {
            grabInteractable.interactionManager.SelectExit(currentInteractor, grabInteractable);
            isGrabbing = false;
        }
    }
}
