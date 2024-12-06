using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class HideControllerWhenHolding : MonoBehaviour
{
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private GameObject controllerObject;
    private string holdingController = ""; // Store whether "Right" or "Left" controller is holding the object

    void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    void OnDestroy()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        var interactor = args.interactorObject;
        controllerObject = interactor.transform.gameObject;

        // Determine if it's the right or left hand grabbing the object based on tag
        if (controllerObject.CompareTag("RightHand"))
        {
            holdingController = "Right";
        }
        else if (controllerObject.CompareTag("LeftHand"))
        {
            holdingController = "Left";
        }

        // Hide the controller when the object is grabbed
        Renderer[] controllerRenderers = controllerObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in controllerRenderers)
        {
            renderer.enabled = false;
        }

        // Make the object kinematic while being held
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    void OnRelease(SelectExitEventArgs args)
    {
        // Show the controller when the object is released
        if (controllerObject != null)
        {
            Renderer[] controllerRenderers = controllerObject.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in controllerRenderers)
            {
                renderer.enabled = true;
            }
        }

        // Restore the object's kinematic and gravity properties
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        controllerObject = null;
        holdingController = "";
    }

    public string GetHoldingController()
    {
        return holdingController;
    }
}
