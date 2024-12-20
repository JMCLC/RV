using UnityEngine;
using UnityEngine.XR;

public class ArmSwingMovement : MonoBehaviour
{
    public XRNode leftHandNode = XRNode.LeftHand;
    public XRNode rightHandNode = XRNode.RightHand;

    public float swingMultiplier = 1.0f; // Adjust movement speed
    public float detectionThreshold = 0.1f; // Minimum swing velocity
    public float maxSpeed = 3.0f; // Maximum movement speed

    public float gravity = -9.8f; // Gravity strength
    public LayerMask groundLayer; // Define which layers count as "ground"

    private CharacterController characterController;
    private Vector3 velocity; // Gravity and movement velocity
    private bool isGrounded;

    // References to HideControllerWhenHolding scripts for both hands
    public HideControllerWhenHolding leftHandHideController;
    public HideControllerWhenHolding rightHandHideController;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogError("CharacterController is required for ArmSwingMovement!");
        }
    }

    private void Update()
    {
        // Check if grounded
        GroundCheck();

        // Apply gravity if not grounded
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Slight downward force to stay on the ground
        }
        else
        {
            velocity.y += gravity * Time.deltaTime; // Apply gravity
        }

        // Check if hands are holding a weapon
        bool isLeftHandHolding = leftHandHideController != null && leftHandHideController.GetHoldingController() == "Left";
        bool isRightHandHolding = rightHandHideController != null && rightHandHideController.GetHoldingController() == "Right";

        // Get velocities for hands that are not holding weapons
        Vector3 leftHandVelocity = isLeftHandHolding ? Vector3.zero : GetVelocityFromXRNode(leftHandNode);
        Vector3 rightHandVelocity = isRightHandHolding ? Vector3.zero : GetVelocityFromXRNode(rightHandNode);

        // Adjust movement speed calculation to double the active hand's contribution
        float leftSwing = isLeftHandHolding ? 0 : Mathf.Abs(leftHandVelocity.z);
        float rightSwing = isRightHandHolding ? 0 : Mathf.Abs(rightHandVelocity.z);
        float totalSwing = isLeftHandHolding || isRightHandHolding
            ? (leftSwing + rightSwing) * 2.0f // Double the active hand's contribution
            : leftSwing + rightSwing; // Normal calculation when both hands are free

        float movementSpeed = totalSwing * swingMultiplier;
        movementSpeed = Mathf.Clamp(movementSpeed, 0, maxSpeed);

        // Determine movement direction
        Vector3 movementDirection = Vector3.zero;
        if (movementSpeed > detectionThreshold)
        {
            movementDirection = GetAverageControllerDirection(isLeftHandHolding, isRightHandHolding);
            movementDirection.y = 0; // Prevent unintended vertical movement
            movementDirection.Normalize();
        }

        // Combine movement and gravity
        Vector3 finalMovement = movementDirection * movementSpeed + velocity;

        // Move the character
        characterController.Move(finalMovement * Time.deltaTime);
    }

    private void GroundCheck()
    {
        // Perform a sphere check at the bottom of the CharacterController
        Vector3 spherePosition = new Vector3(
            transform.position.x,
            transform.position.y - characterController.height / 2 + characterController.radius,
            transform.position.z
        );
        isGrounded = Physics.CheckSphere(spherePosition, characterController.radius, groundLayer, QueryTriggerInteraction.Ignore);
    }

    private Vector3 GetVelocityFromXRNode(XRNode handNode)
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(handNode);
        if (device.isValid && device.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 velocity))
        {
            return velocity;
        }
        return Vector3.zero;
    }

    private Vector3 GetAverageControllerDirection(bool isLeftHandHolding, bool isRightHandHolding)
    {
        Vector3 leftDirection = Vector3.zero;
        Vector3 rightDirection = Vector3.zero;

        if (!isLeftHandHolding)
        {
            InputDevice leftHand = InputDevices.GetDeviceAtXRNode(leftHandNode);
            if (leftHand.isValid && leftHand.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion leftRotation))
            {
                leftDirection = leftRotation * Vector3.forward;
            }
        }

        if (!isRightHandHolding)
        {
            InputDevice rightHand = InputDevices.GetDeviceAtXRNode(rightHandNode);
            if (rightHand.isValid && rightHand.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rightRotation))
            {
                rightDirection = rightRotation * Vector3.forward;
            }
        }

        Vector3 averageDirection = leftDirection + rightDirection;
        if (averageDirection != Vector3.zero)
        {
            averageDirection.Normalize();
        }

        return averageDirection;
    }
}
