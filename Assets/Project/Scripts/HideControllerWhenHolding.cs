using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HideControllerWhileHolding : MonoBehaviour
{
    public GameObject leftControllerVisual;  // Assign in the Inspector (e.g., Left Controller Visual GameObject)
    public GameObject rightControllerVisual; // Assign in the Inspector (e.g., Right Controller Visual GameObject)

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private Rigidbody gunRigidbody;

    void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        gunRigidbody = GetComponent<Rigidbody>();

        // Subscribe to the grab and release events
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    void OnDestroy()
    {
        // Unsubscribe from the events
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        var interactor = args.interactorObject as UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor;
        if (interactor is UnityEngine.XR.Interaction.Toolkit.Interactors.NearFarInteractor)
        {
            // Check for handedness
            if (interactor.transform.CompareTag("LeftHand"))
            {
                leftControllerVisual.SetActive(false);
            }
            else if (interactor.transform.CompareTag("RightHand"))
            {
                rightControllerVisual.SetActive(false);
            }
        }

        // Disable gravity while holding
        gunRigidbody.useGravity = false;
        gunRigidbody.isKinematic = true; // Prevent the Rigidbody from interfering during grab
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        var interactor = args.interactorObject as UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor;

              if (interactor is UnityEngine.XR.Interaction.Toolkit.Interactors.NearFarInteractor)
        {
            // Check for handedness
            if (interactor.transform.CompareTag("LeftHand"))
            {
                leftControllerVisual.SetActive(true);
            }
            else if (interactor.transform.CompareTag("RightHand"))
            {
                rightControllerVisual.SetActive(true);
            }
        }

        // Enable gravity after releasing
        gunRigidbody.useGravity = true;
        gunRigidbody.isKinematic = false; // Let the Rigidbody move freely
    }
}
