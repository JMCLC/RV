using UnityEngine;

public class MoveTarget : MonoBehaviour
{
    public enum Axis { X, Y, Z } // Enum for axis selection
    public Axis moveAxis = Axis.X; // Choose the axis to move on in the Inspector

    public float speed = 2.0f; // Speed of movement
    public float range = 5.0f; // Distance from the starting position

    private Vector3 startPosition;

    void Start()
    {
        // Record the starting position
        startPosition = transform.position;
    }

    void Update()
    {
        // Calculate the offset based on PingPong
        float offset = Mathf.PingPong(Time.time * speed, range * 2) - range;

        // Update position based on selected axis
        Vector3 newPosition = startPosition;
        switch (moveAxis)
        {
            case Axis.X:
                newPosition.x += offset;
                break;
            case Axis.Y:
                newPosition.y += offset;
                break;
            case Axis.Z:
                newPosition.z += offset;
                break;
        }
        transform.position = newPosition;
    }
}

