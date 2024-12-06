using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class VRShoot : MonoBehaviour
{
    public SimpleShoot simpleShoot; // Reference to SimpleShoot script
    private AudioSource audioSource; // AudioSource for shooting sound

    private XRGrabInteractable grabInteractable;
    private bool isHeld = false;
    private bool canShoot = true; // Flag to control shooting
    private HideControllerWhenHolding hideControllerScript; // Reference to HideControllerWhenHolding script

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        audioSource = GetComponent<AudioSource>();
        hideControllerScript = GetComponent<HideControllerWhenHolding>();

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
        isHeld = true;
    }

    void OnRelease(SelectExitEventArgs args)
    {
        isHeld = false;
        canShoot = true; // Reset the shoot flag on release
    }

    void Update()
    {
        if (isHeld && hideControllerScript != null)
        {
            string holdingController = hideControllerScript.GetHoldingController();

            // Get the corresponding controller device based on the holding controller
            InputDevice controllerDevice = holdingController == "Right" ? InputDevices.GetDeviceAtXRNode(XRNode.RightHand) :
                                          holdingController == "Left" ? InputDevices.GetDeviceAtXRNode(XRNode.LeftHand) : default;

            if (controllerDevice.isValid)
            {
                bool isTriggerPressed = controllerDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool isPressed) && isPressed;

                if (isTriggerPressed && canShoot)
                {
                    Shoot(controllerDevice); // Trigger the shooting action for the holding controller
                    canShoot = false; // Prevent continuous shooting while holding the trigger
                }

                // Allow shooting again when trigger is released
                if (!isTriggerPressed)
                {
                    canShoot = true;
                }
            }
        }
    }

    private void Shoot(InputDevice controller)
    {
        if (simpleShoot != null)
        {
            simpleShoot.StartShoot();
        }

        PlayGunshotSound();

        // Trigger Haptic Feedback on the respective controller
        if (controller.isValid)
        {
            SendHapticImpulse(controller);
        }
    }

    private void PlayGunshotSound()
    {
        if (audioSource != null)
        {
            // Play the sound allowing overlapping sounds
            audioSource.PlayOneShot(audioSource.clip);
        }
    }

    private void SendHapticImpulse(InputDevice controller)
    {
        // Define haptic parameters with increased amplitude and duration for stronger feedback
        float amplitude = 1.0f; // Increased intensity of the vibration (0.0 to 1.0)
        float duration = 0.3f;  // Increased duration of the vibration in seconds

        // Send haptic feedback using the InputDevice interface
        if (controller.TryGetHapticCapabilities(out HapticCapabilities capabilities) && capabilities.supportsImpulse)
        {
            uint channel = 0; // Usually, channel 0 is used
            controller.SendHapticImpulse(channel, amplitude, duration);
        }
    }
}
