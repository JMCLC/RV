using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class VRShoot : MonoBehaviour
{
    public SimpleShoot simpleShoot; // Reference to SimpleShoot script
    private AudioSource audioSource; // AudioSource for shooting sound

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private bool isHeld = false;
    private bool canShoot = true; // Flag to control shooting

    void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        audioSource = GetComponent<AudioSource>();

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
        if (isHeld)
        {
            // Get the right or left controller device
            InputDevice rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
            InputDevice leftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

            bool isTriggerPressed = false;
            bool isTriggerPressedRight = rightController.TryGetFeatureValue(CommonUsages.triggerButton, out isTriggerPressed) && isTriggerPressed;
            bool isTriggerPressedLeft = leftController.TryGetFeatureValue(CommonUsages.triggerButton, out isTriggerPressed) && isTriggerPressed;

            if (isTriggerPressedRight && canShoot)
            {
                Shoot(rightController); // Pass the right controller for feedback
                canShoot = false; // Prevent continuous shooting while holding the trigger
            }
            else if (isTriggerPressedLeft && canShoot)
            {
                Shoot(leftController); // Pass the left controller for feedback
                canShoot = false; // Prevent continuous shooting while holding the trigger
            }

            // Allow shooting again when trigger is released
            if (!isTriggerPressedRight && !isTriggerPressedLeft)
            {
                canShoot = true;
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
