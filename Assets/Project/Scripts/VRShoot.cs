using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class VRShoot : MonoBehaviour
{
    public SimpleShoot simpleShoot; // Reference to SimpleShoot script
    private AudioSource audioSource; // AudioSource for shooting sound

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private bool isHeld = false;
    private float lastShootTime = 0f; // Keeps track of when the last shot occurred
    private float shootCooldown = 0.25f; // Minimum time between shots (in seconds)

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
    }

    void Update()
    {
        if (isHeld)
        {
            // Get the right or left controller device
            InputDevice rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
            InputDevice leftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

            bool isTriggerPressed = false;
            if (rightController.TryGetFeatureValue(CommonUsages.triggerButton, out isTriggerPressed) && isTriggerPressed)
            {
                AttemptShoot();
            }
            else if (leftController.TryGetFeatureValue(CommonUsages.triggerButton, out isTriggerPressed) && isTriggerPressed)
            {
                AttemptShoot();
            }
        }
    }

    private void AttemptShoot()
    {
        // Add cooldown logic to prevent double shooting
        if (Time.time - lastShootTime > shootCooldown)
        {
            Shoot();
            lastShootTime = Time.time; // Update the last shot time
        }
    }

    private void Shoot()
    {
        if (simpleShoot != null)
        {
            simpleShoot.StartShoot();
        }

        PlayGunshotSound();
    }

    private void PlayGunshotSound()
    {
        if (audioSource != null)
        {
            // Play the sound allowing overlapping sounds
            audioSource.PlayOneShot(audioSource.clip);
        }
    }
}
